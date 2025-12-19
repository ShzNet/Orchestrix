# Orchestrix

> A distributed job orchestration framework for .NET, designed for high availability and scalability.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 🎯 Overview

Orchestrix is a **distributed background job processing framework** that provides:

- **🔄 Job Scheduling** - Immediate, delayed, cron, and interval-based jobs
- **⚡ High Availability** - Leader/Follower Coordinator cluster with automatic failover
- **📊 Horizontal Scaling** - Scale Workers independently based on workload
- **🔌 Pluggable Infrastructure** - Redis/RabbitMQ/Kafka transport, EF Core/InMemory persistence
- **🎛️ Real-time Monitoring** - SignalR-based admin dashboard

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              ORCHESTRIX SYSTEM                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│    ┌─────────────┐     ┌─────────────┐     ┌─────────────┐                  │
│    │   Client    │     │   Client    │     │   Client    │                  │
│    │ (Your App)  │     │ (Your App)  │     │ (Your App)  │                  │
│    └──────┬──────┘     └──────┬──────┘     └──────┬──────┘                  │
│           │                   │                   │                         │
│           └───────────────────┼───────────────────┘                         │
│                               │                                             │
│                        ┌──────▼──────┐                                      │
│                        │   Service   │                                      │
│                        │     API     │                                      │
│                        └──────┬──────┘                                      │
│                               │                                             │
│    ┌──────────────────────────▼───────────────────────────┐                 │
│    │                   COORDINATOR CLUSTER                │                 │
│    │  ┌─────────┐   ┌─────────┐   ┌─────────┐             │                 │
│    │  │ Leader  │   │Follower │   │Follower │             │                 │
│    │  │Schedule │   │  Events │   │  Events │             │                 │
│    │  │Dispatch │   │         │   │         │             │                 │
│    │  └────┬────┘   └────┬────┘   └────┬────┘             │                 │
│    └───────┼─────────────┼─────────────┼──────────────────┘                 │
│            │             │             │                                    │
│            └─────────────┼─────────────┘                                    │
│                          │ Transport (Redis/RabbitMQ/Kafka)                 │
│                   ┌──────▼──────┐                                           │
│           ┌───────┴───────┬─────┴────────┐                                  │
│           │               │              │                                  │
│           ▼               ▼              ▼                                  │
│       ┌──────┐       ┌──────┐       ┌──────┐                                │
│       │Worker│       │Worker│       │Worker│                                │
│       │  1   │       │  2   │       │  N   │                                │
│       └──────┘       └──────┘       └──────┘                                │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🤖 Development Approach

This project is being developed using **Agentic AI-assisted development** (Antigravity by Google DeepMind). The AI agent helps with:

- 📋 Architecture planning and documentation
- 💻 Code implementation
- 🔍 Code review and refactoring
- 📝 Documentation generation

Human expertise guides design decisions while AI accelerates implementation.

---

## 📦 Installation

```bash
# Core packages
dotnet add package Orchestrix.Coordinator
dotnet add package Orchestrix.Worker

# Infrastructure (choose based on your stack)
dotnet add package Orchestrix.Transport.Redis
dotnet add package Orchestrix.Persistence.EfCore
dotnet add package Orchestrix.Locking.Redis
```

---

## 🚀 Quick Start

### 1. Define a Job Handler

```csharp
[JobHandler(Queue = "email")]
public class SendEmailJob : IJobHandler<SendEmailArgs>
{
    public async Task ExecuteAsync(IJobContext context, SendEmailArgs args)
    {
        await context.LogAsync($"Sending email to {args.To}");
        // Your logic here
        await context.UpdateProgressAsync(100, "Done!");
    }
}

public record SendEmailArgs(string To, string Subject, string Body);
```

### 2. Setup Coordinator

```csharp
// Program.cs - Coordinator Host
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOrchestrixCoordinator(options =>
{
    options.NodeId = "coordinator-1";
})
.UseRedisTransport(redis => redis.ConnectionString = "localhost:6379")
.UseEfCorePersistence(ef => ef.UseSqlServer("..."))
.UseRedisLocking(redis => redis.ConnectionString = "localhost:6379");

await builder.Build().RunAsync();
```

### 3. Setup Worker

```csharp
// Program.cs - Worker Host
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOrchestrixWorker(options =>
{
    options.WorkerName = "worker-1";
    options.Queues = ["email", "default"];
    options.MaxConcurrency = 10;
})
.UseRedisTransport(redis => redis.ConnectionString = "localhost:6379")
.AddJobHandlersFromAssembly(typeof(SendEmailJob).Assembly);

await builder.Build().RunAsync();
```

### 4. Enqueue Jobs

```csharp
// From your application
public class MyService
{
    private readonly IOrchestrixClient _client;

    public async Task SendWelcomeEmail(string email)
    {
        // Immediate job
        await _client.EnqueueAsync<SendEmailJob>(new { To = email, Subject = "Welcome!" });
        
        // Delayed job (1 hour)
        await _client.ScheduleAsync<SendEmailJob>(TimeSpan.FromHours(1), new { ... });
        
        // Recurring job (every day at 9 AM)
        await _client.CreateRecurringAsync<DailyReportJob>("daily-report", "0 9 * * *");
    }
}
```

---

## 📁 Project Structure

```
Orchestrix/
├── src/
│   ├── Orchestrix.Abstractions/          # Core types, interfaces
│   ├── Transport/
│   │   ├── Orchestrix.Transport.Abstractions/
│   │   ├── Orchestrix.Transport.Redis/
│   │   ├── Orchestrix.Transport.RabbitMQ/
│   │   └── Orchestrix.Transport.Kafka/
│   ├── Locking/
│   │   ├── Orchestrix.Locking.Abstractions/
│   │   ├── Orchestrix.Locking.InMemory/
│   │   └── Orchestrix.Locking.Redis/
│   ├── Coordinator/
│   │   ├── Orchestrix.Coordinator.Abstractions/
│   │   ├── Orchestrix.Coordinator.Persistence.Abstractions/
│   │   └── Orchestrix.Coordinator/
│   ├── Persistence/
│   │   ├── Orchestrix.Persistence.InMemory/
│   │   └── Orchestrix.Persistence.EfCore/
│   ├── Orchestrix.Worker/
│   ├── Orchestrix.ControlPanel/
│   ├── Orchestrix.ServiceApi/
│   ├── Orchestrix.Client/
│   └── Orchestrix.Admin/
└── samples/
```

---

## 🔧 Features

| Feature | Status |
|:--------|:-------|
| Core Abstractions | ✅ Completed |
| Transport Abstractions | ✅ Completed |
| Distributed Locking | ✅ Completed |
| **Coordinator Persistence** | ✅ **Completed** |
| Immediate Jobs | 🔄 In Progress |
| Delayed Jobs | 🔄 In Progress |
| Cron Jobs | 🔄 In Progress |
| Interval Jobs | 🔄 In Progress |
| Job Priorities | ✅ Foundation Ready |
| Retry Policies | ✅ Foundation Ready |
| Dead Letter Queue | ✅ Foundation Ready |
| Job Cancellation | 🔲 Planned |
| Progress Tracking | 🔲 Planned |
| Real-time Logs | 🔲 Planned |
| HA Coordinator | 🔄 In Progress |
| Graceful Shutdown | 🔲 Planned |
| Admin Dashboard | 🔲 Planned |

**Legend:**
- ✅ Completed - Feature fully implemented
- 🔄 In Progress - Currently being implemented
- ✅ Foundation Ready - Core types/abstractions ready
- 🔲 Planned - Not yet started

---

## 📚 Documentation

- [Architecture Overview](docs/architecture-coordinator-cluster.MD)
- [Implementation Roadmap](todo.MD)
- [Phase Details](tasks/)

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgments

- Inspired by [Maestro](https://github.com/Netflix/maestro) - Netflix's workflow orchestrator
- Built with assistance from [Antigravity](https://deepmind.google/) - Agentic AI Coding Assistant
