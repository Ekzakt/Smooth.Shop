using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Smooth.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ITokenService tokenService)
        {
            _logger = logger;
            _configuration = configuration;
            _tokenService = tokenService;
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

            var apiUri = $"{_configuration["FlauntApi:Uri"]}/weatherforecasts";
            var token = await _tokenService.GetTokenAsync("flauntapi.read");

            if (token.IsError)
            {

            }

            httpClient.SetBearerToken(token.AccessToken);

            var result = await httpClient.GetAsync(apiUri);

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
