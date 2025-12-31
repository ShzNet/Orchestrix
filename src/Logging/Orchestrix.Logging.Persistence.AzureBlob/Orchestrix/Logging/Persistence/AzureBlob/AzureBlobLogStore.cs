using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using Orchestrix.Logging.Persistence.Entities;

namespace Orchestrix.Logging.Persistence.AzureBlob;

/// <summary>
/// Implementation of <see cref="ILogStore"/> using Azure Blob Storage (Append Blobs).
/// </summary>
public class AzureBlobLogStore : ILogStore
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobLogStore> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobLogStore"/> class.
    /// </summary>
    /// <param name="blobServiceClient">The Blob Service Client.</param>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="logger">The logger.</param>
    public AzureBlobLogStore(BlobServiceClient blobServiceClient, string containerName, ILogger<AzureBlobLogStore> logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task AppendAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = containerClient.GetAppendBlobClient($"{logEntry.JobId}.jsonl");
            await blobClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(logEntry) + Environment.NewLine;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            
            await blobClient.AppendBlockAsync(stream, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to append log to Azure Blob for Job {JobId}", logEntry.JobId);
            // Don't throw to avoid disrupting the job execution? Or throw?
            // Usually logging failure shouldn't crash the app, but might be critical for audit.
            // Rethrowing for now as per interface contract implies success means persisted.
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LogEntry>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetAppendBlobClient($"{jobId}.jsonl");

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                return Array.Empty<LogEntry>();
            }

            var download = await blobClient.DownloadContentAsync(cancellationToken);
            var content = download.Value.Content.ToString();
            
            var logs = new List<LogEntry>();
            using var reader = new StringReader(content);
            while (await reader.ReadLineAsync() is { } line)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try 
                    {
                        var entry = JsonSerializer.Deserialize<LogEntry>(line);
                        if (entry != null) logs.Add(entry);
                    }
                    catch
                    {
                        // Ignore malformed lines
                    }
                }
            }

            return logs;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Failed to retrieve logs from Azure Blob for Job {JobId}", jobId);
             throw;
        }
    }
}
