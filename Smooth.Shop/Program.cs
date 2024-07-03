using Smooth.Shop.Application.Configuration;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Infrastructure.Services;

namespace Smooth.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.Configure<IdentityServerOptions>(builder.Configuration.GetSection(IdentityServerOptions.SectionName));

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
