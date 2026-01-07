using Grpc.Core;
using ProtoBuf.Grpc.Server;

namespace DcmViewer.Common.Services;

public class DcmServiceGrpcServer<TService> : DcmServiceServer<TService>
    where TService : IDcmService
{
    private readonly TService _serviceInstance;
    private readonly Server _server;

    public DcmServiceGrpcServer(TService serviceInstance)
    {
        _serviceInstance = serviceInstance;
        _server = new Server();
        _server.Services.AddCodeFirst(serviceInstance);
        _server.Ports.Add(new ServerPort("localhost", ServiceAttribute.Port, ServerCredentials.Insecure));
    }

    public override async Task Start()
        => await Task.Run(() => _server.Start());

    public override async Task Stop()
        => await _server.ShutdownAsync();

    public override void Dispose()
    {
        try
        {
            _serviceInstance.RequestShutdown();
            _server.ShutdownAsync().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}