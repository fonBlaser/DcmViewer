using DcmViewer.Common.Services;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.DeidentifierService;

[DcmServiceInterface("Deidentifier", "DcmViewer.DeidentifierService.Executable", 10012)]
[Service]
public interface IDcmDeidentifier : IDcmService
{
    
}