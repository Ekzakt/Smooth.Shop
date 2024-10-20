using IdentityModel.Client;

namespace Smooth.Shop.Application.Contracts;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(string scope);
}
