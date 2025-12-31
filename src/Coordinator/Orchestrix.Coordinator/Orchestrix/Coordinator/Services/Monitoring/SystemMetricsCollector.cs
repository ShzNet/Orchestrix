using Microsoft.Extensions.Logging;

namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// Implementation of <see cref="ISystemMetricsCollector"/> using System.Diagnostics.Process.
/// </summary>
public class SystemMetricsCollector : ISystemMetricsCollector
{
    private readonly ILogger<SystemMetricsCollector> _logger;
    
    private TimeSpan _lastTotalProcessorTime = TimeSpan.Zero;
    private DateTimeOffset _lastCpuCollectionTime = DateTimeOffset.MinValue;
    private readonly object _cpuLock = new();

    /// <summary>
    /// Initializes a new instance of <see cref="SystemMetricsCollector"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public SystemMetricsCollector(ILogger<SystemMetricsCollector> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public long GetCpuMillicores()
    {
        lock (_cpuLock)
        {
            try 
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var now = DateTimeOffset.UtcNow;
                var currentTotalProcessorTime = process.TotalProcessorTime;

                if (_lastCpuCollectionTime == DateTimeOffset.MinValue)
                {
                    _lastTotalProcessorTime = currentTotalProcessorTime;
                    _lastCpuCollectionTime = now;
                    return 0;
                }

                var timeDelta = now - _lastCpuCollectionTime;
                var cpuDelta = currentTotalProcessorTime - _lastTotalProcessorTime;

                _lastCpuCollectionTime = now;
                _lastTotalProcessorTime = currentTotalProcessorTime;

                if (timeDelta <= TimeSpan.Zero) return 0;

                // Millicores = (CpuDelta / TimeDelta) * 1000
                // Example: 1 sec CPU in 1 sec Realwall = 1000m (1 core 100%)
                var usage = (cpuDelta.TotalMilliseconds / timeDelta.TotalMilliseconds) * 1000;
                
                // Cap at 0 if negative for some reason
                return (long)Math.Max(0, usage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to collect CPU metrics");
                return 0;
            }
        }
    }

    /// <inheritdoc />
    public long GetMemoryUsageBytes()
    {
        try
        {
            // Working Set is roughly equivalent to "Memory Usage" shown in Task Manager (RSS)
            return System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        }
        catch (Exception ex)
        {
             _logger.LogWarning(ex, "Failed to collect Memory metrics");
             return 0;
        }
    }
}
