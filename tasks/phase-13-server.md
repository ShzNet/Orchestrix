# Phase 13: Server

> Pre-packaged all-in-one server application for Orchestrix.
> Includes Coordinator, Service API, Admin API, and Dashboard in a single deployable package.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Server` | net10.0 | Coordinator, ControlPanel, ServiceApi, Admin |
| `Orchestrix.Dashboard` | React | Vite + React + TypeScript + shadcn/ui |

---

## Concept

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        Orchestrix.Server                                │
│                                                                         │
│   ┌─────────────────────────────────────────────────────────────────┐   │
│   │                    ASP.NET Core Host                            │   │
│   │                                                                 │   │
│   │   ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐     │   │
│   │   │ Coordinator │  │ Service API │  │    Admin API        │     │   │
│   │   │   Service   │  │  /api/jobs  │  │ /admin/* + SignalR  │     │   │
│   │   └─────────────┘  └─────────────┘  └─────────────────────┘     │   │
│   │                                                                 │   │
│   │   ┌─────────────────────────────────────────────────────────┐   │   │
│   │   │              Embedded Dashboard (SPA)                   │   │   │
│   │   │                                                         │   │   │
│   │   │   ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐    │   │   │
│   │   │   │  Jobs   │  │ Workers │  │Schedules│  │  Logs   │    │   │   │
│   │   │   └─────────┘  └─────────┘  └─────────┘  └─────────┘    │   │   │
│   │   │                                                         │   │   │
│   │   └─────────────────────────────────────────────────────────┘   │   │
│   │                                                                 │   │
│   └─────────────────────────────────────────────────────────────────┘   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 13.1 Orchestrix.Server

### Usage
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrchestrixServer(options =>
{
    options.NodeId = "orchestrix-server-1";
    options.Dashboard.Enabled = true;
    options.Dashboard.PathPrefix = "/dashboard";
})
.UseRedisTransport(redis => redis.ConnectionString = "localhost:6379")
.UseEfCorePersistence(ef => ef.UseSqlServer("..."))
.UseRedisLocking(redis => redis.ConnectionString = "localhost:6379");

var app = builder.Build();

app.MapOrchestrixServer();  // Maps all endpoints + Dashboard

app.Run();
```

### Files
- [ ] `ServerOptions.cs`
  ```csharp
  public class ServerOptions : CoordinatorOptions
  {
      public DashboardOptions Dashboard { get; set; } = new();
      public bool EnableServiceApi { get; set; } = true;
      public bool EnableAdminApi { get; set; } = true;
  }
  
  public class DashboardOptions
  {
      public bool Enabled { get; set; } = true;
      public string PathPrefix { get; set; } = "/dashboard";
      public bool RequireAuthorization { get; set; } = false;
      public string? AuthorizationPolicy { get; set; }
  }
  ```

- [ ] `ServiceCollectionExtensions.cs`
- [ ] `EndpointRouteBuilderExtensions.cs`
  ```csharp
  public static IEndpointRouteBuilder MapOrchestrixServer(
      this IEndpointRouteBuilder endpoints)
  {
      endpoints.MapOrchestrixServiceApi();  // /api/jobs, /api/schedules
      endpoints.MapOrchestrixAdmin();       // /admin/*, SignalR hub
      endpoints.MapOrchestrixDashboard();   // /dashboard (SPA)
      return endpoints;
  }
  ```

**Files: 3**

---

## 13.2 Orchestrix.Dashboard (React)

> React SPA embedded in Server package. Built with Vite + React + TypeScript + shadcn/ui.

### Tech Stack
- **React 18** + TypeScript
- **Vite** - Build tool
- **shadcn/ui** - Component library (TailwindCSS + Radix UI)
- **TanStack Query** - Data fetching
- **React Router** - Navigation
- **SignalR Client** - Real-time updates

### Pages
- [ ] `JobsPage.tsx` - Job list, search, filter, actions
- [ ] `JobDetailPage.tsx` - Job details, logs
- [ ] `WorkersPage.tsx` - Worker status, health
- [ ] `SchedulesPage.tsx` - Cron/Interval schedules management
- [ ] `LogsPage.tsx` - Real-time log viewer
- [ ] `SettingsPage.tsx` - Configuration view

### Components
- [ ] `JobCard.tsx` - Job summary card
- [ ] `JobStatusBadge.tsx` - Status visualization
- [ ] `WorkerStatusBadge.tsx` - Worker health indicator
- [ ] `LogViewer.tsx` - Real-time log streaming (SignalR)
- [ ] `JobActionsMenu.tsx` - Cancel, retry, delete
- [ ] `Sidebar.tsx` - Navigation sidebar
- [ ] `Header.tsx` - Top header with search

### Hooks
- [ ] `useJobs.ts` - Job data fetching
- [ ] `useWorkers.ts` - Worker data fetching
- [ ] `useSignalR.ts` - Real-time connection

**Files: ~20**

---

## 13.3 UI Wireframes

### Dashboard Layout
```
+--------------------------------------------------------------------------+
|  ORCHESTRIX DASHBOARD                                          [Admin]   |
+--------------------------------------------------------------------------+
| +--------+                                                               |
| | Dash   |  +-------------------------------------------------------+    |
| | Jobs   |  |                      OVERVIEW                         |    |
| | Workers|  |  +----------+ +----------+ +----------+ +----------+  |    |
| | Sched  |  |  | PENDING  | | RUNNING  | |COMPLETED | | FAILED   |  |    |
| | Logs   |  |  |   125    | |   42     | |  8,432   | |   23     |  |    |
| |        |  |  +----------+ +----------+ +----------+ +----------+  |    |
| |        |  +-------------------------------------------------------+    |
| |        |                                                               |
| |        |  +-------------------------------------------------------+    |
| |        |  |                     RECENT JOBS                       |    |
| |        |  +-------------------------------------------------------+    |
| |        |  | SendEmailJob      user@example.com          Running   |    |
| |        |  | ProcessVideoJob   video-123.mp4             Completed |    |
| |        |  | GenerateReport    Q4-Report                 Pending   |    |
| |        |  | SyncDataJob       external-api              Failed    |    |
| |        |  +-------------------------------------------------------+    |
| +--------+                                                               |
+--------------------------------------------------------------------------+
```

### Jobs List Page
```
+--------------------------------------------------------------------------+
|  JOBS                                                                    |
|  +--------------------------------------------------------------------+  |
|  | Search jobs...                  [Status v] [Queue v] [Date v]      |  |
|  +--------------------------------------------------------------------+  |
|                                                                          |
|  +--------------------------------------------------------------------+  |
|  | Job ID    | Type           | Queue   | Status    | Created    | : |   |
|  +--------------------------------------------------------------------+  |
|  | a1b2c3d4  | SendEmailJob   | email   | Running   | 10:30:00   | : |   |
|  | e5f6g7h8  | ProcessVideo   | video   | Pending   | 10:28:00   | : |   |
|  | i9j0k1l2  | GenerateReport | default | Completed | 10:25:00   | : |   |
|  | m3n4o5p6  | SyncDataJob    | sync    | Failed    | 10:20:00   | : |   |
|  +--------------------------------------------------------------------+  |
|                                                                          |
|  < 1 2 3 4 5 >                                          Showing 1-20     |
+--------------------------------------------------------------------------+
```

### Job Detail Page
```
+--------------------------------------------------------------------------+
|  <- Back to Jobs                                                         |
|                                                                          |
|  +--------------------------------------------------------------------+  |
|  |  SendEmailJob                                      [Cancel] [Retry]|  |
|  |  ID: a1b2c3d4-5678-90ab-cdef-1234567890ab                          |  |
|  |  Queue: email  |  Priority: High  |  Created: 2025-12-18 10:30:00  |  |
|  +--------------------------------------------------------------------+  |
|  |  Status: RUNNING                                                   |  |
|  |  Worker: worker-1  |  Started: 10:30:15  |  Elapsed: 00:02:34      |  |
|  +--------------------------------------------------------------------+  |
|                                                                          |
|  +--------------------------------------------------------------------+  |
|  |  LOGS (Real-time)                                    [Auto-scroll] |  |
|  +--------------------------------------------------------------------+  |
|  |  10:30:15 [INFO]  Starting SendEmailJob...                         |  |
|  |  10:30:16 [INFO]  Loading 100 recipients from database             |  |
|  |  10:30:18 [DEBUG] Batch 1/100: Sending to user1@example.com        |  |
|  |  10:32:45 [INFO]  Sent 67/100 emails                               |  |
|  |  _                                                                 |  |
|  +--------------------------------------------------------------------+  |
+--------------------------------------------------------------------------+
```

### Workers Page
```
+--------------------------------------------------------------------------+
|  WORKERS                                                 Total: 5 Online |
|                                                                          |
|  +---------------+  +---------------+  +---------------+                 |
|  | [OK] worker-1 |  | [OK] worker-2 |  | [OK] worker-3 |                 |
|  | ------------- |  | ------------- |  | ------------- |                 |
|  | Queues: email |  | Queues: video |  | Queues: *     |                 |
|  | Active: 3 job |  | Active: 1 job |  | Active: 5 job |                 |
|  | CPU: 45%      |  | CPU: 78%      |  | CPU: 23%      |                 |
|  | Memory: 512MB |  | Memory: 2.1GB |  | Memory: 256MB |                 |
|  | Last: 5s ago  |  | Last: 3s ago  |  | Last: 2s ago  |                 |
|  +---------------+  +---------------+  +---------------+                 |
|                                                                          |
|  +---------------+  +---------------+                                    |
|  | [!] worker-4  |  | [X] worker-5  |                                    |
|  | ------------- |  | ------------- |                                    |
|  | Status: Drain |  | Status: Off   |                                    |
|  | Active: 0 job |  | Last: 5m ago  |                                    |
|  | Shutting down |  |               |                                    |
|  +---------------+  +---------------+                                    |
+--------------------------------------------------------------------------+
```

**Files: ~20**

---

## 13.4 Docker Image

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "src/Server/Orchestrix.Server/Orchestrix.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Orchestrix.Server.dll"]
```

### Docker Compose (with Workers)
```yaml
version: '3.8'
services:
  orchestrix:
    image: orchestrix/server:latest
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__Redis=redis:6379
      - ConnectionStrings__Database=...
    depends_on:
      - redis
      - postgres
  
  worker:
    image: orchestrix/worker:latest
    deploy:
      replicas: 3
    depends_on:
      - redis
```

---

## Summary
| Project | Files |
|:--------|:------|
| Orchestrix.Server | 3 |
| Orchestrix.Dashboard | 15 |
| Docker | 2 |
| **Total** | **20** |
