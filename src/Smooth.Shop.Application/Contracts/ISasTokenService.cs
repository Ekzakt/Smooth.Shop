using Smooth.Shop.Application.Requests;
using Smooth.Shop.Application.Responses;

namespace Smooth.Shop.Application.Contracts
{
    public interface ISasTokenService
    {
        SasTokenResponse GenerateSasToken(SasTokenRequest sasTokenRequest);
    }
}