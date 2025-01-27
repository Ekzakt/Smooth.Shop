using Azure.Storage.Sas;
using Azure.Storage;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Responses;
using Smooth.Shop.Application.Requests;

namespace Smooth.Shop.Infrastructure.Services;

public class SasTokenService : ISasTokenService
{
    /// <summary>
    /// Generates a SAS token for accessing a specified Azure Blob Storage container.
    /// </summary>
    /// <param name="sasTokenRequest">The request containing storage account details and container name.</param>
    /// <returns>A response containing the SAS token and success status.</returns>
    public SasTokenResponse GenerateSasToken(SasTokenRequest sasTokenRequest)
    {
        try
        {
            var sasToken = GenerateContainerSasToken(
                sasTokenRequest.StorageAccountName,
                sasTokenRequest.StorageAccountKey,
                sasTokenRequest.ContainerName);

            var containerUriWithSas = $"https://{sasTokenRequest.StorageAccountName}.blob.core.windows.net/{sasTokenRequest.ContainerName}?{sasToken}";

            return new SasTokenResponse
            {
                Success = true,
                SasTokon = sasToken,
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
    private string GenerateContainerSasToken(string storageAccountName, string storageAccountKey, string containerName)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            Resource = "c", // "c" means container-level access
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start time
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)    // Expiry time
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.Write |
                                   BlobContainerSasPermissions.Add |
                                   BlobContainerSasPermissions.Create |
                                   BlobContainerSasPermissions.Read);

        var storageSharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

        return sasToken;
    }

    #endregion Helpers
}
