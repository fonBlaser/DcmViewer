using ProtoBuf.Grpc;

namespace DcmViewer.Common.Services;

public class DcmServiceBase : IDcmService
{
    protected CancellationTokenSource ShutdownCts { get; } = new();

    public virtual async Task Connect(CallContext context = default)
    {
        try
        {
            CancellationTokenSource linkedCts 
                = CancellationTokenSource.CreateLinkedTokenSource(
                    context.CancellationToken,
                    ShutdownCts.Token);

            await Task.Delay(Timeout.Infinite, linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public virtual async Task RequestShutdown(CallContext context = default)
    {
        await ShutdownCts.CancelAsync();
    }
}