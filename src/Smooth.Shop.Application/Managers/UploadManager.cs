using Ekzakt.Utilities.Results;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Dtos;
using Smooth.Shop.Domain.Models;
using Smooth.Shop.Application.Mappers;
using Smooth.Shop.Application.Services.FileNameComposer;

namespace Smooth.Shop.Application.Managers;

public class UploadManager
{
    private readonly ISasTokenService _sasTokenService;
    private readonly INewMediumRepo _newMediumRepository;


    public UploadManager(
        ISasTokenService sasTokenService,
        INewMediumRepo newMediumRepository)
    {
        _sasTokenService = sasTokenService;
        _newMediumRepository = newMediumRepository;
    }


    public async Task<Result<NewMediumDto>> ConfirmUploadAsync(ConfirmUploadDto? confirmUploadDto)
    {
        if (confirmUploadDto is null)
        { 
            return Result<NewMediumDto>.Fail($"{nameof(ConfirmUploadDto)} is null.");
        }

        var decomposedFileName = new UploadFileNameComposer()
            .Decompose(confirmUploadDto.OriginalFileName);

        var newMedium = new NewMedium
        {
            UserId = decomposedFileName.UserId,
            ConnectionId = decomposedFileName.ConnectionId,
            OriginalFileName = decomposedFileName.OriginalFileName,
            UploadedFileName = decomposedFileName.UploadedFileName,
            FileSize = confirmUploadDto.FileSize,
            UploadStartedAt = DateTimeOffset.FromUnixTimeMilliseconds(decomposedFileName.UploadStartedAt).UtcDateTime,
            UploadFinishedAt = DateTimeOffset.FromUnixTimeMilliseconds(confirmUploadDto.UploadFinishedAt).UtcDateTime,
            UploadTimeMs = confirmUploadDto.UploadFinishedAt - decomposedFileName.UploadStartedAt,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        try
        {
            await _newMediumRepository.AddAsync(newMedium);

            var newMediumDto = newMedium.ToDto();

            return Result<NewMediumDto>.Success(newMediumDto);
        }
        catch (Exception ex)
        {
            return Result<NewMediumDto>.Fail(ex.Message);
        }
    }
}
