using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.Application.Models;
using System.Text.Json;

namespace Smooth.Shop.Controllers;

public class WeatherForecastsController : Controller
{
    private readonly ILogger<WeatherForecastsController> _logger;
    private readonly IConfiguration _configuration;

    public WeatherForecastsController(ILogger<WeatherForecastsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    [Authorize]
    public async Task<IActionResult> Index()
    {
        using var httpClient = new HttpClient();

        var apiBaseUri = _configuration.GetValue<string>("FlauntApi:BaseUri");
        var apiUri = $"{apiBaseUri}/weatherforecasts";

        //var token = await _tokenService.GetTokenAsync("flauntapi.read");

        try
        {
            _logger.LogInformation("Attempting to retrieve access_token.");
            var token = await HttpContext.GetTokenAsync("access_token");

            if (token == null)
            {
                _logger.LogError("No access_token was found.");
            }

            _logger.LogInformation("An access_token was found.");
            _logger.LogInformation("Attempting to set bearer token.");

            httpClient.SetBearerToken(token ?? string.Empty);

            _logger.LogInformation("Bearer token successfully set.");
            _logger.LogInformation($"Attempting to get data from {apiUri}");

            var result = await httpClient.GetAsync(apiUri);

            result.EnsureSuccessStatusCode();

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
}
