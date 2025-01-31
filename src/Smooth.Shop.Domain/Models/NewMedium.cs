namespace Smooth.Shop.Domain.Models;

#nullable disable

public class NewMedium
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string SessionId { get; set; }

    public string OriginalFileName { get; set; }

    public DateOnly CreatedAt { get; set; }

    public string Status { get; set; }
}
