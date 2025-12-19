using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Locking;
using Orchestrix.Locking.Redis;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering Redis locking services.
/// </summary>
public static class RedisLockingServiceCollectionExtensions
{
    /// <summary>
    /// Configures Redis distributed locking.
    /// </summary>
    /// <param name="builder">The locking builder.</param>
    /// <param name="connectionString">Redis connection string.</param>
    /// <returns>The locking builder for chaining.</returns>
    public static ILockingBuilder UseRedis(
        this ILockingBuilder builder,
        string connectionString)
    {
        return UseRedis(builder, options =>
        {
            options.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Configures Redis distributed locking.
    /// </summary>
    /// <param name="builder">The locking builder.</param>
    /// <param name="configure">Configuration action for Redis lock options.</param>
    /// <returns>The locking builder for chaining.</returns>
    public static ILockingBuilder UseRedis(
        this ILockingBuilder builder,
        Action<RedisLockOptions> configure)
    {
        var options = new RedisLockOptions();
        configure(options);

        // Register Redis connection as singleton
        builder.Services.TryAddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(options.ConnectionString));

        // Register options
        builder.Services.AddSingleton(options);

        // Register lock provider
        builder.Services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();

        return builder;
    }
}
