using System.Text.Json;

namespace Microsoft.Extensions.Caching.Distributed;

/// <summary>
/// Extension methods for IDistributedCache to support object serialization.
/// </summary>
public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    extension(IDistributedCache cache)
    {
        /// <summary>
        /// Gets an object from cache.
        /// </summary>
        public async Task<T?> GetObjectAsync<T>(string key,
            CancellationToken cancellationToken = default) where T : class
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(bytes, DefaultJsonOptions);
        }

        /// <summary>
        /// Sets an object in cache.
        /// </summary>
        public async Task SetObjectAsync<T>(string key,
            T value,
            TimeSpan ttl,
            CancellationToken cancellationToken = default) where T : class
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, DefaultJsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await cache.SetAsync(key, bytes, options, cancellationToken);
        }

        /// <summary>
        /// Sets an object in cache with custom options.
        /// </summary>
        public async Task SetObjectAsync<T>(string key,
            T value,
            DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default) where T : class
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, DefaultJsonOptions);
            await cache.SetAsync(key, bytes, options, cancellationToken);
        }
    }
}
