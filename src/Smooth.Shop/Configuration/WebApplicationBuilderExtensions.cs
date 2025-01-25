using Azure.Identity;
using Smooth.Shared.Configuration;

namespace Smooth.Shop.Configuration;


public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAzureKeyVault(this WebApplicationBuilder builder)
    {

#if !DEBUG

        AzureOptions azureOptions = new();

        builder.Configuration
            .GetSection(AzureOptions.SectionName)
            .Bind(azureOptions);

        var credentialOptions = GetDefaultAzureCredentialOptions(builder);
        var keyVaultUri = azureOptions.KeyVault.VaultUri;

        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential(credentialOptions));

#endif

        return builder;
    }


    #region Helpers

    private static DefaultAzureCredentialOptions GetDefaultAzureCredentialOptions(WebApplicationBuilder builder)
    {
        return new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = true,
            ExcludeInteractiveBrowserCredential = true,
            ExcludeAzurePowerShellCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeAzureCliCredential = !builder.Environment.IsDevelopment(),
            ExcludeManagedIdentityCredential = builder.Environment.IsDevelopment()
        };
    }

    #endregion Helpers
}
