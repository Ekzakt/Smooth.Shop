using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Smooth.Shop.Controllers;

public class AuthenticationController : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var result = await HttpContext.AuthenticateAsync();

        if (result.Succeeded)
        {
            ViewBag.AuthProperties = result.Properties;
        }

        return View();
    }
}
