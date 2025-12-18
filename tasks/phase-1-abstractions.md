# Phase 1: Orchestrix.Abstractions

> Core types, enums, interfaces, and DTOs used throughout the project.

## Project Info
- **Path:** `src/Orchestrix.Abstractions/`
- **Target:** `netstandard2.1`
- **Dependencies:** None (pure abstractions)

---

## Files to Create

### Project File
- [ ] `Orchestrix.Abstractions.csproj`

### Enums
- [ ] `JobStatus.cs` - Job lifecycle statuses
  ```csharp
  public enum JobStatus
  {
      Created = 0,
      Pending = 1,
      Scheduled = 2,
      Dispatched = 3,
      Running = 4,
      Completed = 5,
      Failed = 6,
      TimedOut = 7,
      Cancelled = 8
  }
  ```

- [ ] `JobPriority.cs` - Priority levels
  ```csharp
  public enum JobPriority { Low = 0, Normal = 1, High = 2, Critical = 3 }
  ```

- [ ] `LogLevel.cs` - Log levels
  ```csharp
  public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical }
  ```

### Interfaces
- [ ] `IJobContext.cs` - Job execution context
  ```csharp
  public interface IJobContext
  {
      Guid JobId { get; }
      Guid HistoryId { get; }
      string JobName { get; }
      string Queue { get; }
      int RetryCount { get; }
      int MaxRetries { get; }
      string? CorrelationId { get; }
      CancellationToken CancellationToken { get; }
      
      Task LogAsync(string message, LogLevel level = LogLevel.Information);
      Task UpdateProgressAsync(int percentage, string? message = null);
  }
  ```

- [ ] `IJobHandler.cs` - Job handler contract
  ```csharp
  public interface IJobHandler<TArgs>
  {
      Task ExecuteAsync(IJobContext context, TArgs arguments);
  }
  ```

### DTOs
- [ ] `JobInfo.cs` - Job information record
  ```csharp
  public record JobInfo
  {
      public required Guid Id { get; init; }
      public required string JobType { get; init; }
      public required string Queue { get; init; }
      public required JobStatus Status { get; init; }
      public required JobPriority Priority { get; init; }
      public DateTimeOffset CreatedAt { get; init; }
      public DateTimeOffset? ScheduledAt { get; init; }
      public DateTimeOffset? StartedAt { get; init; }
      public DateTimeOffset? CompletedAt { get; init; }
      public int RetryCount { get; init; }
      public int MaxRetries { get; init; }
      public string? Error { get; init; }
      public string? CorrelationId { get; init; }
  }
  ```

- [ ] `ScheduleInfo.cs` - Schedule information record
  ```csharp
  public record ScheduleInfo
  {
      public required string Id { get; init; }
      public required string JobType { get; init; }
      public required string Queue { get; init; }
      public string? CronExpression { get; init; }
      public TimeSpan? Interval { get; init; }
      public DateTimeOffset? NextRunAt { get; init; }
      public DateTimeOffset? LastRunAt { get; init; }
      public bool IsEnabled { get; init; }
  }
  ```

- [ ] `RetryPolicy.cs` - Retry configuration
  ```csharp
  public record RetryPolicy
  {
      public int MaxRetries { get; init; } = 3;
      public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(10);
      public double BackoffMultiplier { get; init; } = 2.0;
      public TimeSpan MaxDelay { get; init; } = TimeSpan.FromMinutes(10);
  }
  ```

### Attributes
- [ ] `JobHandlerAttribute.cs` - Mark job handler classes
  ```csharp
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class JobHandlerAttribute : Attribute
  {
      public string? Name { get; set; }
      public string Queue { get; set; } = "default";
      public int MaxRetries { get; set; } = 3;
      public int TimeoutSeconds { get; set; } = 300;
  }
  ```

---

## Verification
```bash
dotnet build src/Orchestrix.Abstractions/
```

## Summary
| Type | Count |
|:-----|:------|
| Enums | 3 |
| Interfaces | 2 |
| DTOs | 3 |
| Attributes | 1 |
| **Total Files** | **~10** |


