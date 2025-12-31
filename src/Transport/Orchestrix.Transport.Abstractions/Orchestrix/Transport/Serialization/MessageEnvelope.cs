namespace Orchestrix.Transport.Serialization;

/// <summary>
/// Message envelope containing metadata and payload.
/// </summary>
public class MessageEnvelope
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// The timestamp when the message was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The type of the message contained in the payload.
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// The serialized message payload.
    /// </summary>
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
