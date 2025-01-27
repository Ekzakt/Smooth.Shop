namespace Smooth.Shop.Application.Requests;

#nullable disable

public class SasTokenRequest
{
    public string StorageAccountName { get; set; }

    public string StorageAccountKey { get; set; }

    public string ContainerName { get; set; }
}
