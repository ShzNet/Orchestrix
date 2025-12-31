using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orchestrix.Logging.Persistence.AzureBlob;

/// <summary>
/// Extension methods for configuring Azure Blob logging persistence.
/// </summary>
public static class AzureBlobLoggingServiceCollectionExtensions
{
    /// <summary>
    /// Configures the coordinator to use Azure Blob Storage for job logging.
    /// </summary>
    /// <param name="builder">The logging configuration builder.</param>
    /// <param name="connectionString">The Azure Storage connection string.</param>
    /// <param name="containerName">The container name to store logs in. Check naming rules.</param>
    /// <returns>The logging configuration builder.</returns>
    public static ILoggingConfigurationBuilder UseAzureBlobLogging(this ILoggingConfigurationBuilder builder, string connectionString, string containerName = "orchestrix-job-logs")
    {
        builder.Services.TryAddSingleton(x => new BlobServiceClient(connectionString));
        builder.Services.TryAddScoped<ILogStore>(sp => new AzureBlobLogStore(
            sp.GetRequiredService<BlobServiceClient>(), 
            containerName,
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AzureBlobLogStore>>()
        ));
        
        return builder;
    }
}
