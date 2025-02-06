using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.Application.Configuration;

namespace Smooth.Shop.Controllers;

public class ConfigController : Controller
{
    private readonly ILogger<ConfigController> _logger;
    private readonly IConfiguration _configuration;

    public ConfigController(ILogger<ConfigController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    [HttpGet]
    public IActionResult AllowedFileTypes(CancellationToken cancellationToken)
    {
        var allowedFileTypes = _configuration
            .GetSection(AllowedFileTypeOptions.SECTION_NAME)
            .Get<List<AllowedFileTypeOptions>>();

        if (allowedFileTypes == null)
        {
            return NotFound();
        }

        return Ok(allowedFileTypes);
    }
}
