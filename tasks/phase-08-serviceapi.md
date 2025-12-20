# Phase 8: Service API

> REST API for 3rd party services to integrate with Orchestrix.
> Uses `IControlPanelService` from Phase 7.

## Project
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.ServiceApi` | netstandard2.1 | Orchestrix.ControlPanel, Microsoft.AspNetCore.* |

---

## Folder Structure

```
src/Api/Orchestrix.ServiceApi/
├── Orchestrix.ServiceApi.csproj
└── Orchestrix/
    └── ServiceApi/
        ├── Controllers/
        │   ├── JobsController.cs
        │   └── SchedulesController.cs
        ├── DTOs/
        │   ├── EnqueueRequest.cs
        │   └── EnqueueResult.cs
        ├── ServiceCollectionExtensions.cs
        └── EndpointRouteBuilderExtensions.cs
```

**Namespaces:**
- `Orchestrix.ServiceApi.Controllers` - API controllers
- `Orchestrix.ServiceApi.DTOs` - Request/Response DTOs

---

## Concept

```
┌─────────────────────────────────────────────────────────────────────┐
│                       3rd Party Services                            │
│   ┌─────────────┐   ┌─────────────┐   ┌─────────────┐               │
│   │  Service A  │   │  Service B  │   │  Service C  │               │
│   └──────┬──────┘   └──────┬──────┘   └──────┬──────┘               │
│          │                 │                 │                      │
│          └─────────────────┼─────────────────┘                      │
│                            │ HTTP                                   │
│                            ▼                                        │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.ServiceApi (Phase 8)                │   │
│   │                                                             │   │
│   │  POST /api/jobs          → EnqueueAsync()                   │   │
│   │  DELETE /api/jobs/{id}   → CancelAsync()                    │   │
│   │  GET /api/jobs/{id}      → GetJobAsync()                    │   │
│   │  GET /api/jobs           → GetJobsAsync()                   │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                            │                                        │
│                            ▼                                        │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.ControlPanel (Phase 7)              │   │
│   │              IControlPanelService                           │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Controllers

- [ ] `JobsController.cs`
  ```csharp
  [ApiController]
  [Route("api/jobs")]
  public class JobsController : ControllerBase
  {
      private readonly IControlPanelService _controlPanel;
      
      [HttpPost]
      public async Task<ActionResult<EnqueueResult>> Enqueue([FromBody] EnqueueRequest request)
      {
          var jobId = await _controlPanel.EnqueueAsync(request.JobType, request.Arguments, request.Options);
          return Ok(new EnqueueResult { JobId = jobId });
      }
      
      [HttpGet("{id}")]
      public async Task<ActionResult<JobInfo>> GetJob(Guid id)
          => Ok(await _controlPanel.GetJobAsync(id));
      
      [HttpGet]
      public async Task<ActionResult<PaginatedResult<JobInfo>>> GetJobs([FromQuery] JobQueryOptions options)
          => Ok(await _controlPanel.GetJobsAsync(options));
      
      [HttpDelete("{id}")]
      public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason)
      {
          await _controlPanel.CancelAsync(id, reason);
          return NoContent();
      }
  }
  ```

- [ ] `SchedulesController.cs`
  - POST /api/schedules - Create recurring
  - GET /api/schedules - List
  - DELETE /api/schedules/{id} - Delete
  - POST /api/schedules/{id}/trigger - Trigger now

---

## DTOs

- [ ] `EnqueueRequest.cs`
- [ ] `EnqueueResult.cs`
- [ ] `CreateScheduleRequest.cs`

---

## Registration

- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddOrchestrixServiceApi(this IServiceCollection services)
  {
      services.AddOrchestrixControlPanel(); // Include Phase 7
      return services;
  }
  ```

- [ ] `EndpointRouteBuilderExtensions.cs`
  ```csharp
  public static IEndpointRouteBuilder MapOrchestrixServiceApi(this IEndpointRouteBuilder endpoints)
  {
      endpoints.MapControllers();
      return endpoints;
  }
  ```

---

## Usage

```csharp
// Program.cs
builder.Services.AddOrchestrixServiceApi();
app.MapOrchestrixServiceApi();
```

---

## Unit Tests

- [ ] `JobsControllerTests.cs`
  - Enqueue endpoint
  - Get job endpoint
  - Cancel endpoint

---

## Summary
| Type | Files |
|:-----|:------|
| Controllers | 2 |
| DTOs | 3 |
| Registration | 2 |
| Unit Tests | 1 |
| **Total** | **8** |
