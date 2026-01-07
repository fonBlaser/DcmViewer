namespace DcmViewer.Common.Services;

public abstract class DcmServiceServer<TService> : IDisposable
    where TService : IDcmService
{
    protected DcmServiceInterfaceAttribute ServiceAttribute { get; }

    protected DcmServiceServer()
    {
        ServiceAttribute = DcmServiceInterfaceAttribute.Get<TService>()
                           ?? throw new InvalidOperationException($"The service interface {typeof(TService).FullName} is missing the DcmServiceInterface attribute.");
    }

    public abstract Task Start();
    public abstract Task Stop();
    public abstract void Dispose();
}