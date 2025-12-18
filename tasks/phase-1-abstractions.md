# Phase 1: Orchestrix.Abstractions

> **Status**: ✅ **COMPLETED**  
> Core types, enums, and abstractions for the Orchestrix framework.

## Project Info
- **Path:** `src/Orchestrix.Abstractions/`
- **Target:** `netstandard2.1`
- **Dependencies:** None (pure abstractions)

---

## Folder Structure

```
src/Orchestrix.Abstractions/
├── Orchestrix.Abstractions.csproj
├── Orchestrix/
│   ├── Enums/
│   │   ├── JobStatus.cs
│   │   ├── JobPriority.cs
│   │   └── LogLevel.cs
│   ├── Jobs/
│   │   ├── JobInfo.cs
│   │   └── IJobContext.cs
│   └── Schedules/
│       ├── ScheduleInfo.cs
│       └── RetryPolicy.cs
└── Polyfills/
    ├── IsExternalInit.cs
    ├── RequiredMemberAttribute.cs
    └── SetsRequiredMembersAttribute.cs
```

**Namespaces:**
- `Orchestrix.Enums` - Job status, priority, log levels
- `Orchestrix.Jobs` - Job-related types (JobInfo, IJobContext)
- `Orchestrix.Schedules` - Schedule-related types (ScheduleInfo, RetryPolicy)
- `System.Runtime.CompilerServices` / `System.Diagnostics.CodeAnalysis` - Polyfills (outside Orchestrix/)

> **Note**: 
> - Polyfills are placed outside `Orchestrix/` folder because they use `System.*` namespaces
> - `IJobHandler` and `JobHandlerAttribute` moved to Worker phase (not core abstractions)

---

## Files to Create

### Project File
- [x] `Orchestrix.Abstractions.csproj`

### Enums (in `Orchestrix/Enums/`)
- [x] `JobStatus.cs` - Job lifecycle statuses
  ```csharp
  namespace Orchestrix.Enums;
  
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

- [x] `JobPriority.cs` - Priority levels
  ```csharp
  namespace Orchestrix.Enums;
  
  public enum JobPriority { Low = 0, Normal = 1, High = 2, Critical = 3 }
  ```

- [x] `LogLevel.cs` - Log levels
  ```csharp
  namespace Orchestrix.Enums;
  
  public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical }
  ```

### Jobs (in `Orchestrix/Jobs/`)
- [x] `IJobContext.cs` - Job execution context
  ```csharp
  namespace Orchestrix.Jobs;
  
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

- [x] `JobInfo.cs` - Job information record
  ```csharp
  namespace Orchestrix.Jobs;
  
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

### Schedules (in `Orchestrix/Schedules/`)
- [x] `ScheduleInfo.cs` - Schedule information record
  ```csharp
  namespace Orchestrix.Schedules;
  
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

- [x] `RetryPolicy.cs` - Retry configuration
  ```csharp
  namespace Orchestrix.Schedules;
  
  public record RetryPolicy
  {
      public int MaxRetries { get; init; } = 3;
      public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(10);
      public double BackoffMultiplier { get; init; } = 2.0;
      public TimeSpan MaxDelay { get; init; } = TimeSpan.FromMinutes(10);
  }
  ```

### Polyfills (outside `Orchestrix/`)
- [x] `IsExternalInit.cs` - Enable `init` accessors on netstandard2.1
- [x] `RequiredMemberAttribute.cs` - Enable `required` keyword
- [x] `SetsRequiredMembersAttribute.cs` - Support required members

---

## Verification
```bash
dotnet build src/Orchestrix.Abstractions/
# ✅ Build succeeded - 0 Warning(s) - 0 Error(s)
```

## Summary
| Type | Count |
|:-----|:------|
| Enums | 3 |
| Jobs | 2 |
| Schedules | 2 |
| Polyfills | 3 |
| **Total Files** | **11** |


