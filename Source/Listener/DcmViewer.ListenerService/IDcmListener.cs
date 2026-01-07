using DcmViewer.Common.Services;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.ListenerService;

[DcmServiceInterface("Listener", "DcmViewer.ListenerService.Executable", 10011)]
[Service]
public interface IDcmListener : IDcmService
{
    public IAsyncEnumerable<ListenerEvent> SubscribeToEvents(CallContext context = default);
    public IAsyncEnumerable<TransmissionCompletedEvent> SubscribeToTransmissions(CallContext context = default);
}