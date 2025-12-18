namespace Orchestrix.Transport;

/// <summary>
/// Interface for publishing messages to transport channels.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="channel">The channel name.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<T>(string channel, T message, CancellationToken cancellationToken = default);
}
