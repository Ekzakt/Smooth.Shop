namespace Smooth.Shop.Application.Responses;

#nullable disable

public class SasTokenResponse
{
    public bool Success { get; set; }

    public string SasToken { get; set; }

    public string SasTokenBaseUrl { get; set; }

    public string SasTokenUrl { get; set; } 

    public string Message { get; set; }
}
