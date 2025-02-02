using Smooth.Shop.Application.Dtos;
using Smooth.Shop.Domain.Models;

namespace Smooth.Shop.Application.Mappers;

public static class NewMediumMapper
{
    public static NewMediumDto ToDto(this NewMedium medium)
    {
        return new NewMediumDto
        {
            Id = medium.Id,
            OriginalFileName = medium.OriginalFileName,
            ConnectionId = medium.ConnectionId,
            UserId = medium.UserId.ToString()
        };
    }


    public static NewMedium ToDomain(this NewMediumDto dto)
    {
        return new NewMedium
        {
            Id = dto.Id,
            OriginalFileName = dto.OriginalFileName,
            ConnectionId = dto.ConnectionId,
            UserId = Guid.Parse(dto.UserId)
        };
    }
}
