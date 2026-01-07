using DcmViewer.Common.Services;
using DcmViewer.ListenerService;

namespace DcmViewer.Tests.Integration.Listener;

[Trait("Category", "Integration")]
public abstract class ListenerServiceTests : DcmServiceTestBase<IDcmListener>
{
    public sealed class ListenerServiceThreadTests : ListenerServiceTests
    {
        protected override DcmServiceController<IDcmListener> GetServiceController()
        {
            throw new NotImplementedException();
        }
    };

    public sealed class ListenerServiceProcessTests : ListenerServiceTests
    {
        protected override DcmServiceController<IDcmListener> GetServiceController()
        {
            throw new NotImplementedException();
        }
    };
}