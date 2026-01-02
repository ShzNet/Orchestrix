using Orchestrix.Jobs;
using Orchestrix.Worker.Execution;

/// <summary>
/// Sample job definition with simple arguments.
/// </summary>
public class SampleJob
{
    public string Message { get; set; } = "Hello, World!";
    public int DelaySeconds { get; set; } = 1;
}

/// <summary>
/// Handler for <see cref="SampleJob"/>.
/// </summary>
public class SampleJobHandler : IJobHandler<SampleJob>
{
    private readonly ILogger<SampleJobHandler> _logger;

    public SampleJobHandler(ILogger<SampleJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(SampleJob args, IJobContext context)
    {
        _logger.LogInformation("[SampleJob] Starting job {JobId}: {Message}", context.JobId, args.Message);
        
        // Simulate work
        await Task.Delay(TimeSpan.FromSeconds(args.DelaySeconds), context.CancellationToken);
        
        _logger.LogInformation("[SampleJob] Completed job {JobId}", context.JobId);
    }
}
