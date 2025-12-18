# Phase 12: Sample

> Sample applications demonstrating Orchestrix usage.

## Projects
| Project | Type | Description |
|:--------|:-----|:------------|
| `Orchestrix.Sample.Coordinator` | Console/Worker | Sample Coordinator host |
| `Orchestrix.Sample.Worker` | Console/Worker | Sample Worker host |
| `Orchestrix.Sample.Api` | Web API | Sample API host |
| `Orchestrix.Sample.Jobs` | Class Library | Sample job handlers |

---

## Folder Structure

```
samples/
├── Orchestrix.Sample.Jobs/
│   ├── Orchestrix.Sample.Jobs.csproj
│   └── Orchestrix/
│       └── Sample/
│           └── Jobs/
│               ├── EmailJobHandler.cs
│               ├── ReportJobHandler.cs
│               └── DataProcessingJobHandler.cs
│
├── Orchestrix.Sample.Coordinator/
│   ├── Orchestrix.Sample.Coordinator.csproj
│   ├── Program.cs
│   └── appsettings.json
│
├── Orchestrix.Sample.Worker/
│   ├── Orchestrix.Sample.Worker.csproj
│   ├── Program.cs
│   └── appsettings.json
│
└── Orchestrix.Sample.Api/
    ├── Orchestrix.Sample.Api.csproj
    ├── Program.cs
    └── appsettings.json
```

**Namespaces:**
- `Orchestrix.Sample.Jobs` - Sample job handlers

---

## 12.1 Sample Jobs

- [ ] `SendEmailJob.cs`
  ```csharp
  [JobHandler(Queue = "email", MaxRetries = 3)]
  public class SendEmailJob : IJobHandler<SendEmailArgs>
  {
      public async Task ExecuteAsync(IJobContext context, SendEmailArgs args)
      {
          await context.LogAsync($"Sending email to {args.To}");
          // Simulate work
          await Task.Delay(1000);
          await context.UpdateProgressAsync(100, "Email sent!");
      }
  }
  
  public record SendEmailArgs(string To, string Subject, string Body);
  ```

- [ ] `ProcessVideoJob.cs`
- [ ] `GenerateReportJob.cs`

---

## 12.2 Sample Coordinator

- [ ] `Program.cs`
  ```csharp
  var builder = Host.CreateApplicationBuilder(args);
  
  builder.Services
      .AddOrchestrixCoordinator(options =>
      {
          options.NodeId = "coordinator-1";
      })
      .AddRedisTransport(options => options.ConnectionString = "localhost:6379")
      .AddRedisLocking(options => options.ConnectionString = "localhost:6379")
      .AddEfCorePersistence(options => options.UseSqlServer("..."));
  
  var host = builder.Build();
  await host.RunAsync();
  ```

---

## 12.3 Sample Worker

- [ ] `Program.cs`
  ```csharp
  var builder = Host.CreateApplicationBuilder(args);
  
  builder.Services
      .AddOrchestrixWorker(options =>
      {
          options.WorkerName = "worker-1";
          options.Queues = ["email", "video", "default"];
          options.MaxConcurrency = 10;
      })
      .AddRedisTransport(options => options.ConnectionString = "localhost:6379")
      .AddJobHandlersFromAssembly(typeof(SendEmailJob).Assembly);
  
  var host = builder.Build();
  await host.RunAsync();
  ```

---

## 12.4 Sample API

- [ ] `Program.cs`
  ```csharp
  var builder = WebApplication.CreateBuilder(args);
  
  builder.Services
      .AddOrchestrixServiceApi()
      .AddOrchestrixAdmin()
      .AddEfCorePersistence(options => options.UseSqlServer("..."))
      .AddRedisTransport(options => options.ConnectionString = "localhost:6379");
  
  var app = builder.Build();
  
  app.MapOrchestrixServiceApi();
  app.MapOrchestrixAdmin();
  
  app.Run();
  ```

---

## Docker Compose

- [ ] `docker-compose.yml`
  ```yaml
  version: '3.8'
  services:
    redis:
      image: redis:7-alpine
      ports:
        - "6379:6379"
    
    postgres:
      image: postgres:15-alpine
      environment:
        POSTGRES_DB: orchestrix
        POSTGRES_USER: orchestrix
        POSTGRES_PASSWORD: orchestrix
      ports:
        - "5432:5432"
    
    coordinator:
      build:
        context: .
        dockerfile: samples/Orchestrix.Sample.Coordinator/Dockerfile
      depends_on:
        - redis
        - postgres
    
    worker:
      build:
        context: .
        dockerfile: samples/Orchestrix.Sample.Worker/Dockerfile
      depends_on:
        - redis
      deploy:
        replicas: 3
    
    api:
      build:
        context: .
        dockerfile: samples/Orchestrix.Sample.Api/Dockerfile
      ports:
        - "5000:80"
      depends_on:
        - redis
        - postgres
  ```

---

## Summary
| Project | Files |
|:--------|:------|
| Sample.Jobs | 3 |
| Sample.Coordinator | 1 |
| Sample.Worker | 1 |
| Sample.Api | 1 |
| Docker | 1 |
| **Total** | **7** |
