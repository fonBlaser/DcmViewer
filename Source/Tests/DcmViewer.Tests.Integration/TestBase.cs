using DcmViewer.Common.Config;

namespace DcmViewer.Tests.Integration;

public abstract class TestBase : IDisposable
{
    protected Guid TestId { get; }
    public string TestRootDirectory { get; }
    public DcmViewerConfig Config { get; }

    public TestBase()
    {
        TestId = Guid.NewGuid();
        TestRootDirectory = Path.Combine(Path.GetTempPath(), "DcmViewer", "Tests", "Integration", TestId.ToString("N"));
        Directory.CreateDirectory(TestRootDirectory);
        Config = new DcmViewerConfig
        {
            RootDataDirectory = TestRootDirectory,
            DcmFilesSubdir = "Dcm",
            PngFilesSubdir = "Png",
            DatabaseFileName = "db.sqlite",
            TcpFileReceiverPort = 10004
        };
    }

    public virtual void Dispose()
    {
        if (Directory.Exists(TestRootDirectory))
            Directory.Delete(TestRootDirectory, true);
    }
}