using DcmViewer.Common.Services;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.MonitoringService;

[DcmServiceInterface("Monitoring", "DcmViewer.MonitoringService.Executable", 10010)]
[Service]
public interface IDcmMonitoring : IDcmService
{
    
}