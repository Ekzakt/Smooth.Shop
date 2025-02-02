using System.Data;

namespace Smooth.Shop.Domain.Models;

#nullable disable

public class NewMedium
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; }

    public string ConnectionId { get; set; }

    public string OriginalFileName { get; set; }

    public string UploadedFileName { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadStartedAt { get; set; }

    public DateTime UploadFinishedAt { get; set; }

    public long UploadTimeMs { get; set; }

    public DateTime CreatedAt { get; set; }
}
