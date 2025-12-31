using Microsoft.Extensions.Logging;
using Orchestrix.Logging.Persistence;
using Orchestrix.Logging.Persistence.Entities;

namespace Orchestrix.Coordinator.Services;

/// <summary>
/// A default implementation of <see cref="ILogStore"/> that writes to <see cref="ILogger"/>.
/// </summary>
internal class LoggerLogStore : ILogStore
{
    private readonly ILogger<LoggerLogStore> _logger;

    public LoggerLogStore(ILogger<LoggerLogStore> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task AppendAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        var level = logEntry.Level?.ToLowerInvariant() switch
        {
            "debug" => LogLevel.Debug,
            "info" or "information" => LogLevel.Information,
            "warn" or "warning" => LogLevel.Warning,
            "error" => LogLevel.Error,
            "critical" or "fatal" => LogLevel.Critical,
            _ => LogLevel.Information
        };

        _logger.Log(level, "Job {JobId}: {Message} {Exception}", logEntry.JobId, logEntry.Message, logEntry.Exception ?? "");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<LogEntry>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<LogEntry>>(Array.Empty<LogEntry>());
    }
}
