using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;

namespace Smooth.Shared.Razor.TagHelpers;

[HtmlTargetElement("version")]
public class VersionTagHelper : TagHelper
{
    public string? VersionPrefix { get; set; } = null;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

        if (!string.IsNullOrEmpty(version))
        {
            version = new Version(version).ToString(3);
        }

        if (!string.IsNullOrEmpty(VersionPrefix))
        {
            version = $"{VersionPrefix}{version}".Trim();
        }

        output.TagName = "span";

        output.Content.SetContent($"{version}");
    }
}
