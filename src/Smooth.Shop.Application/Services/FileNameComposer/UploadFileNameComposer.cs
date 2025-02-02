namespace Smooth.Shop.Application.Services.FileNameComposer;

public class UploadFileNameComposer
{
    private const string SEPARATOR = "__";

    public string Compose(string originalFileName, string connectionId, Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(originalFileName);
        ArgumentNullException.ThrowIfNull(connectionId);

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var output =
            $"{unixTime}" +
            $"{SEPARATOR}{userId}" +
            $"{SEPARATOR}{connectionId}" +
            $"{SEPARATOR}{originalFileName.ToLower()}";

        return output;
    }


    public DecomposedUploadFileModel Decompose(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        var parts = fileName.Split(SEPARATOR);

        if (parts.Length != 4)
            throw new ArgumentException("Invalid file name format.");

        var decomposedFileName = new DecomposedUploadFileModel
        (
            UploadStartedAt: long.Parse(parts[0]),
            UserId: Guid.Parse(parts[1]),
            ConnectionId: parts[2],
            OriginalFileName: parts[3],
            UploadedFileName: fileName
        );

        return decomposedFileName;
    }
}
