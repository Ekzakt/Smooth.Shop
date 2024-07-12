using Smooth.Shop.Application.Configuration;
using Smooth.Shop.Bogus;

namespace Smooth.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ProductData>();
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.Cookie.Name = "Smooth.Shop";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("IdentityServer:Authority");
                options.ClientId = builder.Configuration.GetValue<string>("IdentityServer:ClientId");
                options.ClientSecret = builder.Configuration.GetValue<string>("IdentityServer:ClientSecret");

                options.ResponseType = "code";
                options.ResponseMode = "query";
                
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("flauntapi.read");
                
                options.MapInboundClaims = false; // Don't rename claim types

                options.SaveTokens = true;
            });


            var app = builder.Build();

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

            app
                .MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
