namespace DcmViewer.Common.Services;

public class DcmServiceController<TService> : IDisposable where TService : class, IDcmService
{
    private Func<DcmServiceServer<TService>> _serverProvider;
    private DcmServiceGrpcClient<TService>? _client;

    public DcmServiceServer<TService>? Server { get; private set; }
    public TService? Service => _client?.Service;
    public event Action? Disconnected;
    public event Action? Connected;

    public DcmServiceController(Func<DcmServiceServer<TService>> serverProvider)
    {
        _serverProvider = serverProvider;
    }

    public async Task StartOrKeepAlive(CancellationToken ct = default)
    {
        await Task.Run(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    Server = _serverProvider();
                    Server.Start().Wait(ct);

                    _client = new DcmServiceGrpcClient<TService>();
                    Task clientTask = _client.Service.Connect();
                    OnConnected();
                    clientTask.Wait(ct);
                }
                catch
                {
                }
                finally
                {
                    _client?.Dispose();
                    Server?.Dispose();

                    _client = null;
                    Server = null;

                    OnDisconnected();
                }
            }
        }, ct);
    }

    public async Task<TService> GetService(CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (Service != null)
                    return Service;
            }

            throw new OperationCanceledException();
        }, ct);
    }

    public void Dispose()
    {
        _client?.Dispose();
        Server?.Dispose();
    }

    protected virtual void OnDisconnected()
        => Disconnected?.Invoke();

    protected virtual void OnConnected()
        => Connected?.Invoke();
}