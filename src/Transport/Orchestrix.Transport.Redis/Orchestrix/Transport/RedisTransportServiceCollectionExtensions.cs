using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Transport;
using Orchestrix.Transport.Redis;
using Orchestrix.Transport.Serialization;

namespace Orchestrix.Transport;

/// <summary>
/// Extension methods for configuring Redis transport.
/// </summary>
public static class RedisTransportServiceCollectionExtensions
{
    /// <summary>
    /// Configures the coordinator to use Redis for transport.
    /// </summary>
    /// <param name="builder">The transport configuration builder.</param>
    /// <param name="connectionString">The Redis connection string.</param>
    /// <returns>The transport configuration builder.</returns>
    public static ITransportConfigurationBuilder UseRedis(this ITransportConfigurationBuilder builder, string connectionString)
    {
        builder.Services.TryAddSingleton<IMessageSerializer, JsonMessageSerializer>();
        
        builder.Services.TryAddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => 
            StackExchange.Redis.ConnectionMultiplexer.Connect(connectionString));

        builder.Services.AddSingleton<IPublisher, RedisPublisher>();
        builder.Services.AddSingleton<ISubscriber, RedisSubscriber>();

        return builder;
    }
}
