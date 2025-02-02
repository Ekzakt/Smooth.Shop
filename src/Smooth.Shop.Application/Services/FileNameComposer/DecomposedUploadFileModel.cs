namespace Smooth.Shop.Application.Services.FileNameComposer;

public record DecomposedUploadFileModel(
    long UploadStartedAt,
    Guid UserId,
    string ConnectionId,
    string OriginalFileName,
    string UploadedFileName
);