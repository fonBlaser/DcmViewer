using System.Threading.Channels;
using DcmViewer.Common.Services;
using DcmViewer.Data;
using ProtoBuf.Grpc;

namespace DcmViewer.ListenerService.Implementation;

internal class DcmListener : DcmServiceBase, IDcmListener, IDisposable
{
    private readonly TcpFileReceiver _receiver;
    private readonly DcmDbContext _dbContext;
    private readonly Channel<ListenerEvent> _events = Channel.CreateUnbounded<ListenerEvent>();
    private readonly Channel<TransmissionCompletedEvent> _transmissions = Channel.CreateUnbounded<TransmissionCompletedEvent>();

    public DcmListener(TcpFileReceiver receiver, DcmDbContext dbContext)
    {
        _receiver = receiver;
        _dbContext = dbContext;

        _receiver.EventOccurred += HandleReceiverEvent;
        _receiver.TransmissionCompleted += HandleTransmissionCompleted;
    }

    public async IAsyncEnumerable<ListenerEvent> SubscribeToEvents(CallContext context = default)
    {
        CancellationTokenSource linkedCts 
            = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, ShutdownCts.Token);

        await foreach (ListenerEvent ev in _events.Reader.ReadAllAsync(linkedCts.Token))
        {
            yield return ev;
        }
    }

    public async IAsyncEnumerable<TransmissionCompletedEvent> SubscribeToTransmissions(CallContext context = default)
    {
        CancellationTokenSource linkedCts
            = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, ShutdownCts.Token);

        await foreach (TransmissionCompletedEvent ts in _transmissions.Reader.ReadAllAsync(linkedCts.Token))
        {
            yield return ts;
        }
    }

    private void HandleReceiverEvent(ListenerEvent evt)
    {
        //ToDo: log event to DB

        _events.Writer.TryWrite(evt);
    }

    private void HandleTransmissionCompleted(TransmissionCompletedEvent transmission)
    {
        //ToDo: save file to DB
        //ToDo: update transmissionId with record key

        _transmissions.Writer.TryWrite(transmission);
    }

    public void Dispose()
    {
        RequestShutdown().Wait();
    }
}
