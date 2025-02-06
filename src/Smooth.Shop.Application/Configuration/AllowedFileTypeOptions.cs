namespace Smooth.Shop.Application.Configuration;

#nullable disable

public class AllowedFileTypeOptions
{
    public const string SECTION_NAME = "AllowedFileTypes";

    public string FileProcessorName { get; set; }

    public long MaxFileSize { get; set; }

    public string[] AllowedExtensions { get; set; } = [];
}
