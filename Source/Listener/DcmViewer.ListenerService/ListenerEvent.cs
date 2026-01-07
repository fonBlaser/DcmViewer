using ProtoBuf;

namespace DcmViewer.ListenerService;

[ProtoContract]
public record ListenerEvent
{
    [ProtoMember(1)]
    public required ListenerEventType EventType { get; init; }

    [ProtoMember(2)]
    public string Message { get; init; }
}