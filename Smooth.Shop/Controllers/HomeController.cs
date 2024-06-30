using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Smooth.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> WeatherForecasts()
        {
            using var httpClient = new HttpClient();

            var result = await httpClient.GetAsync("https://localhost:7084/weatherforecasts");

            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<List<WeatherForecastDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(jsonData);
            }

            throw new InvalidOperationException("Could not get content.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
