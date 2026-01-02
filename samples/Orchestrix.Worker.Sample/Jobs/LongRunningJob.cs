using Orchestrix.Jobs;
using Orchestrix.Worker.Execution;

/// <summary>
/// Long running job for testing timeout and cancellation.
/// </summary>
public class LongRunningJob
{
    public int DurationSeconds { get; set; } = 30;
    public bool ShouldFail { get; set; } = false;
}

/// <summary>
/// Handler for <see cref="LongRunningJob"/>.
/// </summary>
public class LongRunningJobHandler : IJobHandler<LongRunningJob>
{
    private readonly ILogger<LongRunningJobHandler> _logger;

    public LongRunningJobHandler(ILogger<LongRunningJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(LongRunningJob args, IJobContext context)
    {
        _logger.LogInformation(
            "[LongRunningJob] Starting job {JobId}, duration: {Duration}s, shouldFail: {ShouldFail}",
            context.JobId, args.DurationSeconds, args.ShouldFail);

        var progress = 0;
        var intervalMs = args.DurationSeconds * 1000 / 10;

        for (var i = 0; i < 10; i++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            await Task.Delay(intervalMs, context.CancellationToken);
            progress += 10;
            
            _logger.LogInformation("[LongRunningJob] Job {JobId} progress: {Progress}%", context.JobId, progress);
        }

        if (args.ShouldFail)
        {
            throw new InvalidOperationException("Job configured to fail");
        }

        _logger.LogInformation("[LongRunningJob] Completed job {JobId}", context.JobId);
    }
}
