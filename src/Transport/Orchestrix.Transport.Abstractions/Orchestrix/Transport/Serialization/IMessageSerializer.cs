namespace Orchestrix.Transport.Serialization;

/// <summary>
/// Interface for serializing and deserializing messages.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes a message to bytes.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="message">The message to serialize.</param>
    /// <returns>The serialized bytes.</returns>
    byte[] Serialize<T>(T message);

    /// <summary>
    /// Deserializes bytes to a message.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="data">The serialized bytes.</param>
    /// <returns>The deserialized message, or null if deserialization fails.</returns>
    T? Deserialize<T>(byte[] data);

    /// <summary>
    /// Deserializes bytes to a message of the specified type.
    /// </summary>
    /// <param name="data">The serialized bytes.</param>
    /// <param name="type">The message type.</param>
    /// <returns>The deserialized message, or null if deserialization fails.</returns>
    object? Deserialize(byte[] data, Type type);
}
