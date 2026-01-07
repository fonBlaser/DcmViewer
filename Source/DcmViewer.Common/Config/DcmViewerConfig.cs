namespace DcmViewer.Common.Config;

public record DcmViewerConfig
{
    public required string RootDataDirectory { get; init; } 
        = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            "DcmViewer");

    public required int TcpFileReceiverPort { get; init; } = 104;
    public required string DcmFilesSubdir { get; init; } = "Dcm";
    public string FullDcmDirectory => Path.Combine(RootDataDirectory, DcmFilesSubdir);

    public required string PngFilesSubdir { get; init; } = "Png";

    public required string DatabaseFileName { get; init; } = "db.sqlite";
    
}