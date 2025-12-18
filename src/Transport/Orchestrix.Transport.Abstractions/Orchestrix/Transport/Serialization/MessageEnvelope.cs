namespace Orchestrix.Transport.Serialization;

/// <summary>
/// Message envelope containing metadata and payload.
/// </summary>
public class MessageEnvelope
{
    public Guid MessageId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
