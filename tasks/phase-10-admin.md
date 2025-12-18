# Phase 10: Admin

> Admin API + Real-time monitoring dashboard.
> Full access to all Orchestrix features + SignalR for live updates.

## Project
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Admin` | netstandard2.1 | Orchestrix.ControlPanel, Microsoft.AspNetCore.*, Microsoft.AspNetCore.SignalR |

---

## Concept

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Admin Dashboard                              │
│                                                                     │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │  Real-time Job Monitor | Worker Status | Cluster Health     │   │
│   │  ┌───────────────────────────────────────────────────────┐  │   │
│   │  │ Job-123: Running ████████░░ 80%                       │  │   │
│   │  │ Job-456: Pending                                      │  │   │
│   │  │ Worker-A: Active (3 jobs)                             │  │   │
│   │  └───────────────────────────────────────────────────────┘  │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                            │                                        │
│              ┌─────────────┴─────────────┐                          │
│              │ HTTP           │ WebSocket                           │
│              ▼                ▼                                     │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.Admin (Phase 10)                    │   │
│   │                                                             │   │
│   │  REST API (Full access)      SignalR Hub                    │   │
│   │  ├── Jobs CRUD               ├── JobStatusUpdated           │   │
│   │  ├── Schedules CRUD          ├── WorkerStatusUpdated        │   │
│   │  ├── Workers                 ├── ClusterEvent               │   │
│   │  └── Cluster                 └── LogReceived                │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                            │                                        │
│                            ▼                                        │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.ControlPanel (Phase 7)              │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Controllers (Full Admin Access)

- [ ] `AdminJobsController.cs`
  - GET /admin/jobs - List with full filter
  - GET /admin/jobs/{id} - Details with logs
  - DELETE /admin/jobs/{id} - Force cancel
  - POST /admin/jobs/{id}/retry - Retry failed
  - DELETE /admin/jobs/bulk - Bulk delete

- [ ] `AdminSchedulesController.cs`
  - Full CRUD
  - Enable/disable
  - Trigger

- [ ] `AdminWorkersController.cs`
  - GET /admin/workers
  - POST /admin/workers/{id}/drain

- [ ] `AdminClusterController.cs`
  - GET /admin/cluster/status
  - GET /admin/cluster/nodes
  - POST /admin/cluster/nodes/{id}/drain

---

## SignalR Hub

- [ ] `AdminHub.cs`
  ```csharp
  public class AdminHub : Hub
  {
      public async Task JoinJobMonitor(Guid jobId)
          => await Groups.AddToGroupAsync(Context.ConnectionId, $"job:{jobId}");
      
      public async Task LeaveJobMonitor(Guid jobId)
          => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"job:{jobId}");
      
      public async Task JoinDashboard()
          => await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
  }
  ```

- [ ] `IAdminNotifier.cs`
  ```csharp
  public interface IAdminNotifier
  {
      Task NotifyJobStatusChangedAsync(JobInfo job);
      Task NotifyJobProgressAsync(Guid jobId, int progress, string? message);
      Task NotifyJobLogAsync(Guid jobId, LogEntry log);
      Task NotifyWorkerStatusChangedAsync(WorkerInfo worker);
      Task NotifyClusterEventAsync(string eventType, object data);
  }
  ```

- [ ] `SignalRAdminNotifier.cs`

---

## Registration

- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddOrchestrixAdmin(this IServiceCollection services)
  {
      services.AddOrchestrixControlPanel();
      services.AddScoped<IAdminNotifier, SignalRAdminNotifier>();
      services.AddSignalR();
      return services;
  }
  ```

- [ ] `EndpointRouteBuilderExtensions.cs`
  ```csharp
  public static IEndpointRouteBuilder MapOrchestrixAdmin(this IEndpointRouteBuilder endpoints)
  {
      endpoints.MapControllers();
      endpoints.MapHub<AdminHub>("/admin/hub");
      return endpoints;
  }
  ```

---

## Usage

```csharp
// Program.cs
builder.Services.AddOrchestrixAdmin();
app.MapOrchestrixAdmin();

// JavaScript client
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/admin/hub")
    .build();

connection.on("JobStatusUpdated", (job) => updateJobUI(job));
connection.on("WorkerStatusUpdated", (worker) => updateWorkerUI(worker));
await connection.start();
await connection.invoke("JoinDashboard");
```

---

## Unit Tests

- [ ] `AdminControllerTests.cs`
  - Job management endpoints
  - Worker management endpoints

---

## Summary
| Type | Files |
|:-----|:------|
| Controllers | 4 |
| SignalR | 3 |
| Registration | 2 |
| Unit Tests | 1 |
| **Total** | **10** |
