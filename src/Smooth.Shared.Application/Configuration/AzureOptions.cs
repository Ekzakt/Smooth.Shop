namespace Smooth.Shared.Application.Configuration;

public class AzureOptions
{
    public const string SectionName = "Azure";

    public AzureKeyVaultOptions KeyVault { get; set; } = new();

    public AzureStorageOptions AzureStorage { get; set; } = new();

    public AzureDefaultsOptions Defaults { get; set; } = new();
}
