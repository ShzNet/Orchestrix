namespace Orchestrix.Transport;

/// <summary>
/// Interface for subscribing to transport channels.
/// </summary>
public interface ISubscriber
{
    /// <summary>
    /// Subscribes to a channel.
    /// Handler returns true to keep subscription, false to auto-unsubscribe.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="channel">The channel name.</param>
    /// <param name="handler">The message handler. Returns true to continue, false to unsubscribe.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubscribeAsync<T>(
        string channel,
        Func<T, Task<bool>> handler,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from a channel.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    Task UnsubscribeAsync(string channel);

    /// <summary>
    /// Closes a channel and cleans up resources.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CloseChannelAsync(string channel, CancellationToken cancellationToken = default);
}
