namespace Microsoft.Extensions.DependencyInjection;

using Orchestrix.Locking;
using Orchestrix.Locking.Redis;
using StackExchange.Redis;

/// <summary>
/// Extension methods for registering Redis locking services.
/// </summary>
public static class RedisLockingServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis distributed locking services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">Redis connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRedisLocking(
        this IServiceCollection services,
        string connectionString)
    {
        return AddRedisLocking(services, options =>
        {
            options.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Adds Redis distributed locking services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for Redis lock options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRedisLocking(
        this IServiceCollection services,
        Action<RedisLockOptions> configure)
    {
        var options = new RedisLockOptions();
        configure(options);

        // Register Redis connection as singleton
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(options.ConnectionString));

        // Register options
        services.AddSingleton(options);

        // Register lock provider
        services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();

        return services;
    }

    /// <summary>
    /// Configures Redis distributed locking.
    /// </summary>
    /// <param name="options">The locking options.</param>
    /// <param name="connectionString">Redis connection string.</param>
    /// <returns>The locking options for chaining.</returns>
    public static LockingOptions UseRedis(
        this LockingOptions options,
        string connectionString)
    {
        return UseRedis(options, redisOptions =>
        {
            redisOptions.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Configures Redis distributed locking.
    /// </summary>
    /// <param name="options">The locking options.</param>
    /// <param name="configure">Configuration action for Redis lock options.</param>
    /// <returns>The locking options for chaining.</returns>
    public static LockingOptions UseRedis(
        this LockingOptions options,
        Action<RedisLockOptions> configure)
    {
        options.Services.AddRedisLocking(configure);
        return options;
    }
}
