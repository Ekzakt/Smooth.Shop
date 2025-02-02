namespace Smooth.Shop.Application.Requests;

#nullable disable

public class ConfirmUploadRequest
{
    public string UploadedFileName { get; set; }

    public long FileSize { get; set; }

    public long UploadFinishedAt { get; set; }
}
