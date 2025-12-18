namespace Orchestrix.Transport;

using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Transport.Serialization;

/// <summary>
/// Options for configuring transport behavior.
/// Used in nested configuration pattern: services.AddCoordinator(opt => opt.Transport.UseRedis(...))
/// </summary>
public class TransportOptions
{
    /// <summary>
    /// Gets the service collection for registering transport services.
    /// Used by extension methods (UseRedis, UseRabbitMQ, etc.)
    /// </summary>
    public IServiceCollection Services { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the channel prefix for all transport channels.
    /// Default is "orchestrix".
    /// </summary>
    public string ChannelPrefix { get; set; } = "orchestrix";

    /// <summary>
    /// Gets or sets the message serializer.
    /// Default is JsonMessageSerializer.
    /// </summary>
    public IMessageSerializer? Serializer { get; set; }

    /// <summary>
    /// Gets or sets the default timeout for operations.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
