using Smooth.Shop.Application.Dtos;
using Smooth.Shop.Application.Requests;
using Smooth.Shop.Domain.Models;

namespace Smooth.Shop.Application.Mappers;

public static class ConfirmUploadMapper
{
    public static ConfirmUploadDto? ToDto(this ConfirmUploadRequest request)
    {
        if (request == null)
            return null;

        return new ConfirmUploadDto
        {
            OriginalFileName = request.UploadedFileName,
            FileSize = request.FileSize,
            UploadFinishedAt = request.UploadFinishedAt
        };
    }

}
