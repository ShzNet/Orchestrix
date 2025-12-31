namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// Service for collecting system metrics (CPU, Memory).
/// </summary>
public interface ISystemMetricsCollector
{
    /// <summary>
    /// Gets the current CPU usage in millicores.
    /// </summary>
    /// <returns>CPU usage in millicores (1000m = 1 core).</returns>
    long GetCpuMillicores();

    /// <summary>
    /// Gets the current memory usage in bytes.
    /// </summary>
    /// <returns>Memory usage in bytes (Working Set).</returns>
    long GetMemoryUsageBytes();
}
