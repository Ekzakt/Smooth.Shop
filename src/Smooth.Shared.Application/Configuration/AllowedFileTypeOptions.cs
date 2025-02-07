namespace Smooth.Shared.Application.Configuration;

#nullable disable

public class AllowedFileTypeOptions
{
    public string FileProcessorName { get; set; }

    public long MaxFileSize { get; set; }

    public string[] AllowedExtensions { get; set; } = [];
}
