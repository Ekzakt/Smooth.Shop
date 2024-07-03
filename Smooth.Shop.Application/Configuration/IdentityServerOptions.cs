namespace Smooth.Shop.Application.Configuration;

#nullable disable

public class IdentityServerOptions
{
    public const string SectionName = "IdentityServer";

    public string DiscoveryUrl { get; init; }    

    public string ClientName { get; init; }

    public string ClientSecret { get; init; }

    public bool UseHttps { get; init; } = true;
}
