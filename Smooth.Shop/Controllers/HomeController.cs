using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Models;
using Smooth.Shop.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Smooth.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> WeatherForecasts()
        {
            using var httpClient = new HttpClient();

            var apiBaseUri = _configuration.GetValue<string>("FlauntApi:BaseUri");
            var apiUri = $"{apiBaseUri}/weatherforecasts";

            //var token = await _tokenService.GetTokenAsync("flauntapi.read");

            var token = await HttpContext.GetTokenAsync("access_token");
            httpClient.SetBearerToken(token ?? string.Empty);

            try
            {
                var result = await httpClient.GetAsync(apiUri);
                var jsonString = await result.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<List<WeatherForecastDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get content: {Exception}", ex);

                return View();
            }
        }


        [Authorize]
        public async Task<IActionResult> AuthCookie()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (result.Succeeded)
            {
                ViewBag.AuthProperties = result.Properties;
            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
