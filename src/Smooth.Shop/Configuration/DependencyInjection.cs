using Azure.Identity;

namespace Smooth.Shop.Configuration;

public static class DependencyInjection
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

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        CorsOptions options = new();
        builder.Configuration
            .GetSection(CorsOptions.SectionName)
            .Bind(options);

        var origins = options?.AllowedOrigins;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: CorsOptions.POLICY_NAME,
                policy =>
                {
                    policy.WithOrigins(origins ?? Array.Empty<string>());
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
        });

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
