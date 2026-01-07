using DcmViewer.Common.Services;

namespace DcmViewer.Tests.Integration;

public abstract class DcmServiceTestBase<TService> : TestBase
    where TService : class, IDcmService
{
    private  DcmServiceController<TService>? _serviceController;

    protected DcmServiceController<TService>? ServiceController
        => _serviceController ??= GetServiceController();

    protected abstract DcmServiceController<TService> GetServiceController();

    public override void Dispose()
    {
        _serviceController?.Dispose();
        base.Dispose();
    }
}