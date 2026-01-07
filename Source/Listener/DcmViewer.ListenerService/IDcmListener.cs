using DcmViewer.Common.Services;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.ListenerService;

[DcmServiceInterface("Listener", "DcmViewer.ListenerService.Executable", 10011)]
[Service]
public interface IDcmListener : IDcmService
{
}