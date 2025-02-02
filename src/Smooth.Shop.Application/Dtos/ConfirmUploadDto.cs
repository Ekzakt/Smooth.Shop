namespace Smooth.Shop.Application.Dtos;

#nullable disable

public class ConfirmUploadDto
{
    public string OriginalFileName { get; set; }

    public long FileSize { get; set; }

    public long UploadFinishedAt { get; set; }
}
