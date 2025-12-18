# Phase 9: Client SDK

> Client library for calling Service API (Phase 8).
> For applications that want to enqueue jobs from another service.

## Project
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Client` | netstandard2.1 | Orchestrix.Abstractions, System.Net.Http.Json |

---

## Concept

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Your Application                             │
│                                                                     │
│   var client = serviceProvider.GetRequiredService<IOrchestrixClient>();
│   var jobId = await client.EnqueueAsync<SendEmailJob>(new { To = "..." });
│                                                                     │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.Client (Phase 9)                    │   │
│   │                                                             │   │
│   │  → HTTP POST /api/jobs                                      │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                            │                                        │
│                            ▼ HTTP                                   │
│   ┌─────────────────────────────────────────────────────────────┐   │
│   │              Orchestrix.ServiceApi (Phase 8)                │   │
│   └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Interface

- [ ] `IOrchestrixClient.cs`
  ```csharp
  public interface IOrchestrixClient
  {
      // Enqueue
      Task<Guid> EnqueueAsync<TJob>(object? arguments = null, EnqueueOptions? options = null, CancellationToken ct = default);
      Task<Guid> ScheduleAsync<TJob>(TimeSpan delay, object? arguments = null, CancellationToken ct = default);
      
      // Recurring
      Task<string> CreateRecurringAsync<TJob>(string scheduleId, string cronExpression, object? arguments = null, CancellationToken ct = default);
      Task<string> CreateRecurringAsync<TJob>(string scheduleId, TimeSpan interval, object? arguments = null, CancellationToken ct = default);
      
      // Cancel
      Task CancelAsync(Guid jobId, string? reason = null, CancellationToken ct = default);
      
      // Query
      Task<JobInfo?> GetJobAsync(Guid id, CancellationToken ct = default);
      Task<JobStatus> GetJobStatusAsync(Guid id, CancellationToken ct = default);
  }
  ```

---

## Implementation

- [ ] `OrchestrixClient.cs`
  ```csharp
  public class OrchestrixClient : IOrchestrixClient
  {
      private readonly HttpClient _httpClient;
      
      public OrchestrixClient(HttpClient httpClient)
      {
          _httpClient = httpClient;
      }
      
      public async Task<Guid> EnqueueAsync<TJob>(object? arguments = null, EnqueueOptions? options = null, CancellationToken ct = default)
      {
          var request = new { JobType = typeof(TJob).FullName, Arguments = arguments, Options = options };
          var response = await _httpClient.PostAsJsonAsync("/api/jobs", request, ct);
          response.EnsureSuccessStatusCode();
          var result = await response.Content.ReadFromJsonAsync<EnqueueResult>(ct);
          return result.JobId;
      }
      
      // ... other methods
  }
  ```

---

## Options

- [ ] `OrchestrixClientOptions.cs`
  ```csharp
  public class OrchestrixClientOptions
  {
      public string BaseUrl { get; set; } = "http://localhost:5000";
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
  }
  ```

---

## Registration

- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddOrchestrixClient(this IServiceCollection services, Action<OrchestrixClientOptions> configure)
  {
      var options = new OrchestrixClientOptions();
      configure(options);
      
      services.AddHttpClient<IOrchestrixClient, OrchestrixClient>(client =>
      {
          client.BaseAddress = new Uri(options.BaseUrl);
          client.Timeout = options.Timeout;
      });
      
      return services;
  }
  ```

---

## Usage

```csharp
// Program.cs
builder.Services.AddOrchestrixClient(options =>
{
    options.BaseUrl = "http://orchestrix-api:5000";
});

// In your service
public class MyService
{
    private readonly IOrchestrixClient _client;
    
    public async Task DoSomething()
    {
        var jobId = await _client.EnqueueAsync<ProcessVideoJob>(new { VideoId = 123 });
    }
}
```

---

## Unit Tests

- [ ] `OrchestrixClientTests.cs`
  - EnqueueAsync (mocked HTTP)
  - CancelAsync
  - GetStatusAsync
  - Error handling

---

## Summary
| Type | Files |
|:-----|:------|
| Interface | 1 |
| Implementation | 1 |
| Options | 1 |
| Registration | 1 |
| Unit Tests | 1 |
| **Total** | **5** |
