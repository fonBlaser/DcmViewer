using ProtoBuf;

namespace DcmViewer.ListenerService;

[ProtoContract]
public record TransmissionCompletedEvent
{
    [ProtoMember(1)]
    public required long TransmissionId { get; init; }

    [ProtoMember(2)]
    public required DateTime StartTime { get; init; }

    [ProtoMember(3)] 
    public required string FileName { get; init; }

    [ProtoMember(4)]
    public required long FileSize { get; init; }
}