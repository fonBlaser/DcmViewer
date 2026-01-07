using DcmViewer.Common.Services;
using DcmViewer.Data;
using DcmViewer.ListenerService;
using DcmViewer.ListenerService.Implementation;

namespace DcmViewer.Tests.Integration.Listener;

[Trait("Category", "Integration")]
public abstract class ListenerServiceTests : DcmServiceTestBase<IDcmListener>
{
    protected TcpFileReceiver Receiver { get; }
    protected IDcmListener Listener { get; }
    protected DcmDbContext DataContext { get; }

    public ListenerServiceTests()
    {
        DatabaseInitializer.Initialize(Config.FullDatabaseFilePath);
        DataContext = new DcmDbContext(Config.FullDatabaseFilePath);

        Receiver = new TcpFileReceiver(Config);
        Receiver.Start();

        Listener = new DcmListener(Receiver, DataContext);
    }

    [Fact]
    public void Check()
    {
        Listener.Connect();
    }

    public override void Dispose()
    {
        Listener.RequestShutdown().Wait();
        Receiver.Stop().Wait();
        DataContext.Dispose();
        base.Dispose();
    }

    public sealed class ListenerServiceThreadTests : ListenerServiceTests
    {
        protected override DcmServiceController<IDcmListener> GetServiceController()
        {
            return new DcmServiceController<IDcmListener>(() => new DcmServiceGrpcServer<IDcmListener>(Listener));
        }
    };

    public sealed class ListenerServiceProcessTests : ListenerServiceTests
    {
        protected override DcmServiceController<IDcmListener> GetServiceController()
        {
            return new DcmServiceController<IDcmListener>(() => new DcmServiceProcessServer<IDcmListener>());
        }
    };
}