using Azure.Identity;
using Ekzakt.FileManager.AzureBlob.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Smooth.Shared.Application.Configuration;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Managers;
using Smooth.Shop.Configuration;
using Smooth.Shop.Data;
using Smooth.Shop.FakeData;
using Smooth.Shop.Hubs;
using Smooth.Shop.Infrastructure.Data;
using Smooth.Shop.Infrastructure.Repos;
using Smooth.Shop.Infrastructure.Services;

namespace Smooth.Shop;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var environment = builder.Environment;

        IdentityModelEventSource.ShowPII = environment.IsDevelopment(); ;

        builder.Services.AddSingleton<ProductData>();
        builder.Services.AddEkzaktFileManagerAzure();

        builder.Services
            .AddOptions<AzureStorageOptions>()
            .BindConfiguration(AzureStorageOptions.SectionName);

        builder.AddCors();

        builder.Services.AddDbContext<SmoothWebDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SmoothShopConnectionString"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Smooth.Shop.Infrastructure");
                }
            )
        );

        builder.Services.AddScoped<UploadManager>();
        builder.Services.AddScoped<ISasTokenService, SasTokenService>();

        builder.Services.AddScoped<INewMediumRepo, NewMediumRepo>();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.Configure<RouteOptions>(routeOptions =>
        {
            routeOptions.LowercaseUrls = true;
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();

        builder.Services
                .AddAzureClients(clientBuilder => {
                    clientBuilder
                        .UseCredential(new DefaultAzureCredential());
                    clientBuilder
                        .AddBlobServiceClient(builder.Configuration.GetSection(AzureStorageOptions.SectionName));
                    clientBuilder
                        .ConfigureDefaults(builder.Configuration.GetSection(AzureDefaultsOptions.SectionName));
                });

        builder.AddAzureKeyVault();

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 256;
        });

#if DEBUG
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024; // 1 GB
        });
#endif

        builder.Services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerConnectionString")));

        builder.Services.AddDataProtection()
            .PersistKeysToDbContext<IdentityDbContext>()
            .SetApplicationName("SmoothSensation.SharedCookie");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = ".AspNet.SharedCookie";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.Domain = builder.Configuration["IdentityServer:CookieDomain"];
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            options.SlidingExpiration = true;
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = builder.Configuration["IdentityServer:Authority"];
            options.ClientId = builder.Configuration["IdentityServer:ClientId"];
            options.ClientSecret = builder.Configuration["IdentityServer:ClientSecret"];
            options.RequireHttpsMetadata = !environment.IsDevelopment();
            options.SaveTokens = false;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.ResponseMode = OpenIdConnectResponseMode.Query;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.UsePkce = true;
            options.MapInboundClaims = false;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.Scope.Add("flauntapi.read");
        });


        var app = builder.Build();

        app.UseForwardedHeaders();
        app.UseCors(CorsOptions.POLICY_NAME);

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthorization();

        app.MapHub<UploadHub>("/uploadHub");
        app
            .MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
