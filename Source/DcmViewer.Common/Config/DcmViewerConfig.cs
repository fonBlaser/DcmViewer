namespace DcmViewer.Common.Config;

public record DcmViewerConfig
{
    public required string ConfigFileName { get; init; } = "DcmViewerConfig.json";
    public required string RootDataDirectory { get; init; } 
        = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            "DcmViewer");

    public required string DcmFilesSubdir { get; init; } = "Dcm";
    public required string PngFilesSubdir { get; init; } = "Png";

    public required string DatabaseFileName { get; init; } = "db.sqlite";
}