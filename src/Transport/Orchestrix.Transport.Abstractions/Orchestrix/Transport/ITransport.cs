namespace Orchestrix.Transport;

/// <summary>
/// Main transport interface combining publisher and subscriber.
/// </summary>
public interface ITransport
{
    /// <summary>
    /// Gets the message publisher.
    /// </summary>
    IPublisher Publisher { get; }

    /// <summary>
    /// Gets the message subscriber.
    /// </summary>
    ISubscriber Subscriber { get; }
}
