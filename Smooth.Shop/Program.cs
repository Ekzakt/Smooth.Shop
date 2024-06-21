using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smooth.Shop.Data;

namespace Smooth.Shop;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\EricJansen"))
            .SetApplicationName("SharedCookieApp");

        builder.Services.ConfigureApplicationCookie(options => {
            options.Cookie.Name = ".AspNet.SharedCookie";
        });

        // Cookies
        //builder.Services.AddDataProtection()
        //    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\EricJansen"))
        //    .SetApplicationName("SmoothApp");

        //builder.Services.ConfigureApplicationCookie(options =>
        //{
        //    //options.ClaimsIssuer = "SharedCookieApp";
        //    options.Cookie.Name = ".AspNet.SmoothCookie";
        //    options.Cookie.Path = "/";
        //    options.Cookie.HttpOnly = true;
        //    options.Cookie.IsEssential = true;
        //    options.Cookie.Domain = "localhost";
        //    options.Cookie.SameSite = SameSiteMode.Lax;
        //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //    options.ExpireTimeSpan = TimeSpan.FromDays(1);
        //});


        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services
            .AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();


        builder.Services.AddControllersWithViews();

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }


    private static void ChangeCookieDomain(AppendCookieContext appendCookieContext, DeleteCookieContext deleteCookieContext)
    {
        if (appendCookieContext != null)
        {
            // Change the domain of all cookies
            //appendCookieContext.CookieOptions.Domain = ".abp.io";

            // Change the domain of the specific cookie
            if (appendCookieContext.CookieName == ".AspNetCore.Culture")
            {
                appendCookieContext.CookieOptions.Domain = ".smooth";
            }
        }

        if (deleteCookieContext != null)
        {
            // Change the domain of all cookies
            //appendCookieContext.CookieOptions.Domain = ".abp.io";

            // Change the domain of the specific cookie
            if (deleteCookieContext.CookieName == ".AspNetCore.Culture")
            {
                deleteCookieContext.CookieOptions.Domain = ".smooth";
            }
        }
    }
}