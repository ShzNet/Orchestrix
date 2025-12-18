# Phase 5: Worker Core

> Worker service for executing jobs.

## Project
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Worker` | netstandard2.1 | Abstractions, Transport.Abstractions, Microsoft.Extensions.Hosting |

---

## Folder Structure

```
src/Worker/Orchestrix.Worker/
├── Orchestrix.Worker.csproj
└── Orchestrix/
    └── Worker/
        ├── WorkerService.cs
        ├── IWorkerService.cs
        ├── WorkerOptions.cs
        ├── WorkerInfo.cs
        ├── WorkerState.cs
        ├── ServiceCollectionExtensions.cs
        ├── Execution/
        │   ├── IJobExecutor.cs
        │   ├── JobExecutor.cs
        │   └── JobContext.cs
        ├── Registry/
        │   ├── IJobHandlerRegistry.cs
        │   └── JobHandlerRegistry.cs
        ├── Consumer/
        │   └── JobConsumer.cs
        ├── Heartbeat/
        │   └── HeartbeatService.cs
        ├── Lifecycle/
        │   ├── GracefulShutdownOptions.cs
        │   └── WorkerDrainService.cs
        └── Cancellation/
            └── CancellationHandler.cs
```

**Namespaces:**
- `Orchestrix.Worker` - Core worker services
- `Orchestrix.Worker.Execution` - Job execution logic
- `Orchestrix.Worker.Registry` - Job handler registry
- `Orchestrix.Worker.Consumer` - Job consumer
- `Orchestrix.Worker.Heartbeat` - Heartbeat service
- `Orchestrix.Worker.Lifecycle` - Graceful shutdown
- `Orchestrix.Worker.Cancellation` - Job cancellation

---

## 5.1 Core

- [ ] `WorkerOptions.cs`
  ```csharp
  public class WorkerOptions
  {
      public string WorkerId { get; set; } = Guid.NewGuid().ToString("N");
      public string WorkerName { get; set; } = Environment.MachineName;
      public string[] Queues { get; set; } = ["default"];
      public int MaxConcurrency { get; set; } = 10;
      public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);
  }
  ```

- [ ] `WorkerInfo.cs`
- [ ] `WorkerState.cs` - Enum: Starting, Running, Draining, Stopped
- [ ] `IWorkerService.cs` / `WorkerService.cs` : BackgroundService
- [ ] `ServiceCollectionExtensions.cs`

---

## 5.2 Execution

- [ ] `IJobExecutor.cs`
- [ ] `JobExecutor.cs`
  - Resolve handler from DI
  - Create JobContext
  - Execute with timeout
  - Catch exceptions, update status
- [ ] `JobContext.cs` - IJobContext implementation
  - Publish logs to `job.{jobId}.logs`
  - Publish status to `job.{jobId}.status`

---

## 5.3 Handler Registry

- [ ] `IJobHandlerRegistry.cs`
- [ ] `JobHandlerRegistry.cs`
  - Auto-discover handlers via reflection
  - Map JobType → Handler type

---

## 5.4 Consumer

- [ ] `JobConsumer.cs` : BackgroundService
  - Subscribe to `job.dispatch.{queue}` for each queue
  - Use SemaphoreSlim for concurrency control
  - Dispatch to JobExecutor

---

## 5.5 Heartbeat

- [ ] `HeartbeatService.cs` : BackgroundService
  - Publish to `worker.heartbeat` every N seconds
  - Include: WorkerId, Status, ActiveJobs, Queues

---

## 5.6 Lifecycle & Graceful Scale Down

> See also: [`docs/architecture-coordinator-cluster.MD`](../docs/architecture-coordinator-cluster.MD) for Coordinator graceful shutdown

### Graceful Scale Down Flow
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                       WORKER GRACEFUL SCALE DOWN FLOW                       │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. Worker receives SIGTERM                                                 │
│     ┌────────────────┐                                                      │
│     │     Worker     │ ◄── SIGTERM                                          │
│     │   (Draining)   │                                                      │
│     │                │ Running: Job-A, Job-B                                │
│     └───────┬────────┘                                                      │
│             │                                                               │
│  2. Enter DRAINING state                                                    │
│     • Unsubscribe from job.dispatch.{queue} (stop accepting new jobs)       │
│     • Send heartbeat with Status = DRAINING                                 │
│     • Notify Coordinator this worker is shutting down                       │
│             │                                                               │
│  3. Cancel all running jobs                                                 │
│     • Trigger CancellationToken for all running jobs                        │
│     • Jobs are cancelled, NOT waited for completion                         │
│     • Publish job status = Cancelled to Coordinator                         │
│             │                                                               │
│  4. Enter STOPPING state                                                    │
│     • Wait for cancellation to propagate (ShutdownTimeout: 30s)             │
│     • Force kill if jobs don't respond to cancellation                      │
│             │                                                               │
│  5. Publish final heartbeat with Status = STOPPED                           │
│     • Coordinator will re-dispatch cancelled jobs                           │
│             │                                                               │
│  6. Shutdown complete                                                       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

> **Note**: Worker does NOT wait for jobs to complete. Jobs are CANCELLED immediately.
> Coordinator is responsible for re-dispatching cancelled jobs to other workers.

### Files
- [ ] `WorkerState.cs`
  ```csharp
  public enum WorkerState
  {
      Starting,
      Running,
      Draining,   // Notifying Coordinator, about to cancel jobs
      Stopping,   // Cancelling jobs, waiting for cleanup
      Stopped
  }
  ```
- [ ] `GracefulShutdownOptions.cs`
  ```csharp
  public class GracefulShutdownOptions
  {
      public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);
  }
  ```
- [ ] `WorkerShutdownService.cs`
  - Handle SIGTERM
  - Transition: Running → Draining → Stopping → Stopped
  - Cancel all running jobs
  - Publish job status updates

---

## 5.7 Cancellation

- [ ] `CancellationHandler.cs`
  - Subscribe to `job.cancel`
  - Cancel running job if matches

---

## Unit Tests

- [ ] `JobExecutorTests.cs`
  - Execute handler
  - Timeout handling
  - Exception handling
  - Cancellation
- [ ] `HandlerRegistryTests.cs`
  - Handler discovery
  - Type mapping
  - Attribute reading

---

## Summary
| Section | Files |
|:--------|:------|
| Core | 5 |
| Execution | 3 |
| Handler Registry | 2 |
| Consumer | 1 |
| Heartbeat | 1 |
| Lifecycle | 2 |
| Cancellation | 1 |
| Unit Tests | 2 |
| **Total** | **17** |
