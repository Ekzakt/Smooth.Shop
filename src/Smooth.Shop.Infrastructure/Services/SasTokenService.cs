using Azure.Storage.Sas;
using Azure.Storage;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Responses;
using Smooth.Shop.Application.Requests;
using Microsoft.Extensions.Options;
using Smooth.Shared.Configuration;

namespace Smooth.Shop.Infrastructure.Services;

public class SasTokenService : ISasTokenService
{
    private readonly AzureStorageOptions _azureStorageOptions;

    public SasTokenService(IOptions<AzureStorageOptions> azureStorageOptions)
    {
        _azureStorageOptions = azureStorageOptions.Value;
    }


    /// <summary>
    /// Generates a SAS token for accessing a specified Azure Blob Storage container.
    /// </summary>
    /// <param name="sasTokenRequest">The request containing storage account details and container name.</param>
    /// <returns>A response containing the SAS token and success status.</returns>
    public SasTokenResponse GenerateSasToken(SasTokenRequest sasTokenRequest)
    {
        try
        {
            var sasToken = GenerateContainerSasToken(_azureStorageOptions.ContainerName);
            var sasTokenBaseUrl = $"{_azureStorageOptions.ServiceUri}/{_azureStorageOptions.ContainerName}/";
            var sasTokenUrl = $"{sasTokenBaseUrl}{sasTokenRequest.FileName}?{sasToken}";

#if DEBUG
            if (sasTokenUrl.StartsWith("https://"))
            {
                sasTokenUrl = sasTokenUrl.Replace("https://", "http://");
            }
#endif
            return new SasTokenResponse
            {
                Success = true,
                SasToken = sasToken,
                SasTokenBaseUrl = sasTokenBaseUrl,
                SasTokenUrl = sasTokenUrl,
                Message = "SasToken created successfully."
            };
        }
        catch (Exception ex)
        {
            return new SasTokenResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    #region Helpers

    /// <summary>
    /// Helper method to generate a SAS token for a specified container.
    /// </summary>
    /// <param name="storageAccountName">The name of the Azure Storage account.</param>
    /// <param name="storageAccountKey">The key of the Azure Storage account.</param>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A SAS token string.</returns>
    private string GenerateContainerSasToken(string containerName)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            Resource = "c", // "c" means container-level access
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start time
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(5)    // Expiry time
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.Write |
                                   BlobContainerSasPermissions.Add |
                                   BlobContainerSasPermissions.Create);

        var storageSharedKeyCredential = new StorageSharedKeyCredential(_azureStorageOptions.AccountName, _azureStorageOptions.AccountKey);
        var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

        //var sasTokenUrl = $"{storageAccountName}"

        return sasToken;
    }

    #endregion Helpers
}
