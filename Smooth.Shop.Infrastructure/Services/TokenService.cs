using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Smooth.Shop.Application.Configuration;
using Smooth.Shop.Application.Contracts;
using System.Reflection.Metadata;

namespace Smooth.Shop.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IdentityServerOptions _options;
    private readonly DiscoveryDocumentResponse _discoveryDocument;


    public TokenService(
        ILogger<TokenService> logger, 
        IOptions<IdentityServerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _discoveryDocument = GetDiscoveryDocument(_options.DiscoveryUrl);
    }


    public async Task<TokenResponse> GetTokenAsync(string scope)
    {
        using var httpClient = new HttpClient();

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Scope = scope,
            Address = _discoveryDocument.TokenEndpoint,
            ClientId = _options.ClientName,
            ClientSecret = _options.ClientSecret
        });

        if (tokenResponse.IsError)
        {
            _logger.LogError("Unable to get token: {TokenResponse}.", tokenResponse.Exception.Message);
            throw new InvalidOperationException("Unable to get discovery document.");
        }

        return tokenResponse;
    }


    #region Helpers

    public DiscoveryDocumentResponse GetDiscoveryDocument(string discoveryUrl)
    {
        if (discoveryUrl == null)
        {
            throw new ArgumentNullException(nameof(discoveryUrl));
        }

        using var httpClient = new HttpClient();

        var document = httpClient.GetDiscoveryDocumentAsync(discoveryUrl).Result;

        if (document.IsError)
        {
            _logger.LogError("Unable to get discovery document: {DiscoveryDocumentError}.", document.Error);
            throw new InvalidOperationException("Unable to get discovery document.");
        }

        return document;
    }

    #endregion Helpers
}
