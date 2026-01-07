using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DcmViewer.Common.Services;

public class DcmServiceGrpcClient<TService> : IDisposable where TService : class, IDcmService
{
    private GrpcChannel _channel;

    public TService Service { get; }

    public DcmServiceGrpcClient()
    {
        DcmServiceInterfaceAttribute? attr = DcmServiceInterfaceAttribute.Get<TService>();
        if (attr is null)
            throw new InvalidOperationException($"The service interface {typeof(TService).FullName} is missing the DcmServiceInterface attribute.");

        _channel = GrpcChannel.ForAddress($"http://localhost:{attr.Port}", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
        Service = _channel.CreateGrpcService<TService>();
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}