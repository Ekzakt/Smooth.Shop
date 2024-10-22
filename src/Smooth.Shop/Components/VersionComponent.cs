using Microsoft.AspNetCore.Mvc;
using System.Reflection;

public class VersionComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? prefix = null)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        if (!string.IsNullOrEmpty(version))
        {
            version = new Version(version).ToString(3);
        }

        if (!string.IsNullOrEmpty(prefix))
        {
            version = $"{prefix}{version}".Trim();
        }

        return View("Default", version);
    }
}