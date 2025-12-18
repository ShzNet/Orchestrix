# Phase 7: ControlPanel

> Core business logic library for job management.
> Communicates with Persistence to enqueue jobs and with Coordinator for execution/monitoring.

## Project
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.ControlPanel` | netstandard2.1 | Coordinator.Persistence.Abstractions, Transport.Abstractions |

---

## Concept

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Application Layer                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│   ┌─────────────┐   ┌─────────────┐   ┌─────────────┐               │
│   │ Service API │   │    Admin    │   │   WPF App   │               │
│   │  (Phase 8)  │   │ (Phase 10)  │   │  (custom)   │               │
│   └──────┬──────┘   └──────┬──────┘   └──────┬──────┘               │
│          │                 │                 │                      │
│          └─────────────────┼─────────────────┘                      │
│                            │                                        │
│                            ▼                                        │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.ControlPanel (Phase 7)              │   │
│   │                                                             │   │
│   │  IControlPanelService                                       │   │
│   │  ├── EnqueueAsync()      → IJobStore.CreateAsync()          │   │
│   │  ├── CancelAsync()       → Transport → Coordinator          │   │
│   │  ├── GetJobsAsync()      → IJobStore.QueryAsync()           │   │
│   │  ├── GetJobStatusAsync() → IJobStore / Transport            │   │
│   │  └── MonitorAsync()      → Transport → Coordinator          │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                            │                                        │
│              ┌─────────────┴─────────────┐                          │
│              ▼                           ▼                          │
│   ┌───────────────────┐       ┌───────────────────┐                 │
│   │     Persistence    │       │    Coordinator    │                 │
│   │  (IJobStore...)   │       │ (via Transport)   │                 │
│   └───────────────────┘       └───────────────────┘                 │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Service Interface

- [ ] `IControlPanelService.cs`
  ```csharp
  public interface IControlPanelService
  {
      // Enqueue jobs - write directly to storage
      Task<Guid> EnqueueAsync<TJob>(object? arguments = null, EnqueueOptions? options = null, CancellationToken ct = default);
      Task<Guid> ScheduleAsync<TJob>(TimeSpan delay, object? arguments = null, CancellationToken ct = default);
      Task<string> CreateRecurringAsync<TJob>(string scheduleId, string cronExpression, object? arguments = null, CancellationToken ct = default);
      Task<string> CreateRecurringAsync<TJob>(string scheduleId, TimeSpan interval, object? arguments = null, CancellationToken ct = default);
      
      // Cancel - communicate with Coordinator via Transport
      Task CancelAsync(Guid jobId, string? reason = null, CancellationToken ct = default);
      
      // Query - from storage
      Task<JobInfo?> GetJobAsync(Guid id, CancellationToken ct = default);
      Task<PaginatedResult<JobInfo>> GetJobsAsync(JobQueryOptions options, CancellationToken ct = default);
      Task<IReadOnlyList<LogEntry>> GetJobLogsAsync(Guid jobId, CancellationToken ct = default);
      
      // Schedules
      Task<ScheduleInfo?> GetScheduleAsync(string id, CancellationToken ct = default);
      Task<PaginatedResult<ScheduleInfo>> GetSchedulesAsync(ScheduleQueryOptions options, CancellationToken ct = default);
      Task DeleteScheduleAsync(string id, CancellationToken ct = default);
      Task TriggerScheduleAsync(string id, CancellationToken ct = default);
      
      // Monitor - communicate with Coordinator
      Task<IReadOnlyList<WorkerInfo>> GetWorkersAsync(CancellationToken ct = default);
      Task<ClusterStatus> GetClusterStatusAsync(CancellationToken ct = default);
  }
  ```

- [ ] `ControlPanelService.cs` - Implementation

---

## Options & Models

- [ ] `ControlPanelOptions.cs`
- [ ] `EnqueueOptions.cs`
- [ ] `JobQueryOptions.cs`
- [ ] `ScheduleQueryOptions.cs`
- [ ] `PaginatedResult.cs`
- [ ] `ClusterStatus.cs`
- [ ] `WorkerInfo.cs`

---

## Registration

- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddOrchestrixControlPanel(this IServiceCollection services)
  {
      services.AddScoped<IControlPanelService, ControlPanelService>();
      return services;
  }
  ```

---

## Unit Tests

- [ ] `ControlPanelServiceTests.cs`
  - Enqueue job
  - Cancel job
  - Query jobs
  - Trigger schedule

---

## Summary
| Type | Files |
|:-----|:------|
| Service | 2 |
| Options/Models | 7 |
| Registration | 1 |
| Unit Tests | 1 |
| **Total** | **11** |
