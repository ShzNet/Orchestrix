namespace Orchestrix.Worker;

/// <summary>
/// Runtime configuration received from Coordinator.
/// Updated after successful registration.
/// </summary>
public class WorkerRuntimeConfig
{
    /// <summary>
    /// Whether registration with Coordinator was successful.
    /// </summary>
    public bool IsRegistered { get; internal set; }

    /// <summary>
    /// Heartbeat interval configured by Coordinator.
    /// </summary>
    public TimeSpan HeartbeatInterval { get; internal set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default job timeout configured by Coordinator.
    /// </summary>
    public TimeSpan DefaultJobTimeout { get; internal set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Worker timeout (considered dead if no heartbeat).
    /// </summary>
    public TimeSpan WorkerTimeout { get; internal set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Event raised when configuration is received from Coordinator.
    /// </summary>
    public event EventHandler? ConfigurationReceived;

    /// <summary>
    /// Signals that configuration has been received.
    /// </summary>
    internal void OnConfigurationReceived()
    {
        IsRegistered = true;
        ConfigurationReceived?.Invoke(this, EventArgs.Empty);
    }
}
