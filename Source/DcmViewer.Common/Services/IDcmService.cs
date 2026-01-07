using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.Common.Services;

[Service]
public interface IDcmService
{
    public Task Connect(CallContext context = default);
    public Task RequestShutdown(CallContext context = default);
}