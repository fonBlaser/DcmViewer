using DcmViewer.Common.Services;
using ProtoBuf.Grpc.Configuration;

namespace DcmViewer.ExtractorService;

[DcmServiceInterface("Extractor", "DcmViewer.ExtractorService.Executable", 10013)]
[Service]
public interface IDcmExtractor : IDcmService
{
}