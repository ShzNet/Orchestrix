# Orchestrix - Distributed Job Scheduling System

> Orchestrix is a distributed background job scheduling and execution system, designed for high availability and scalability.

---

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              ORCHESTRIX ARCHITECTURE                              │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                   │
│   ┌─────────────────┐        ┌─────────────────┐        ┌─────────────────┐      │
│   │   Client API    │        │   Admin API     │        │   Dashboard     │      │
│   │   (Enqueue)     │        │   (Manage)      │        │   (Monitor)     │      │
│   └────────┬────────┘        └────────┬────────┘        └────────┬────────┘      │
│            │                          │                          │                │
│            └──────────────────────────┼──────────────────────────┘                │
│                                       │                                           │
│                                       ▼                                           │
│   ┌───────────────────────────────────────────────────────────────────────────┐   │
│   │                         MESSAGE BROKER                                     │   │
│   │              (Redis Streams / RabbitMQ / Kafka)                           │   │
│   └───────────────────────────────────────────────────────────────────────────┘   │
│                          ▲                    │                                   │
│                          │                    ▼                                   │
│   ┌──────────────────────┴────────────────────────────────────────────────────┐   │
│   │                         COORDINATOR CLUSTER                                │   │
│   │  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐                  │   │
│   │  │ Coordinator 1 │  │ Coordinator 2 │  │ Coordinator 3 │                  │   │
│   │  │   (LEADER)    │  │  (Follower)   │  │  (Follower)   │                  │   │
│   │  └───────────────┘  └───────────────┘  └───────────────┘                  │   │
│   │         │                                                                  │   │
│   │         └──────────────────────┬───────────────────────────────────────┐  │   │
│   │                                │                                       │  │   │
│   │                                ▼                                       │  │   │
│   │                      ┌─────────────────────┐                           │  │   │
│   │                      │      DATABASE       │                           │  │   │
│   │                      │ (PostgreSQL / SQL)  │                           │  │   │
│   │                      └─────────────────────┘                           │  │   │
│   └────────────────────────────────────────────────────────────────────────┘  │   │
│                                       │                                           │
│                                       ▼ (dispatch via Message Broker)             │
│   ┌───────────────────────────────────────────────────────────────────────────┐   │
│   │                           WORKER POOL                                      │   │
│   │  ┌─────────────┐   ┌─────────────┐   ┌─────────────┐   ┌─────────────┐    │   │
│   │  │  Worker A   │   │  Worker B   │   │  Worker C   │   │  Worker D   │    │   │
│   │  │  (video)    │   │  (email)    │   │  (video)    │   │  (general)  │    │   │
│   │  └─────────────┘   └─────────────┘   └─────────────┘   └─────────────┘    │   │
│   │                                                                            │   │
│   │  Workers communicate with Coordinator via Message Broker only              │   │
│   │  (No direct database access)                                               │   │
│   └───────────────────────────────────────────────────────────────────────────┘   │
│                                                                                   │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Core Components

### 2.1 Coordinator Cluster

The Coordinator is the central orchestration hub of the system, running as a cluster with leader election.

| Role | Responsibility |
|:-----|:---------------|
| **Leader** | Scan schedules, create jobs, dispatch, monitor timeouts |
| **Follower** | Handle job events (status, logs), support load balancing |

**Key Features:**
- Leader Election via Distributed Lock
- Follower Coordination (Job Assignment Channel)
- Graceful Scale Down (Job Handoff with Hybrid ACK)
- Crash Recovery (Orphan Job Detection)

### 2.2 Worker

Workers receive jobs from the Coordinator and execute them, reporting status/logs.

**Capabilities:**
- Multi-queue subscription
- Concurrent job execution
- Graceful drain on shutdown
- Heartbeat reporting

### 2.3 Transport

Abstraction layer for message broker, supporting:
- Redis Streams
- RabbitMQ
- Kafka

### 2.4 Persistence

Storage for job state, schedules, logs:
- Entity Framework Core (PostgreSQL, SQL Server, SQLite)
- In-Memory (testing)

---

## 3. Communication Patterns

### 3.1 Channels Overview

| Channel | Type | Purpose |
|:--------|:-----|:--------|
| `orchestrix.job.dispatch.{queue}` | Queue | Dispatch jobs to workers |
| `orchestrix.job.cancel` | Queue | Cancel running jobs |
| `orchestrix.job.assigned` | Queue (Competing) | Assign job ownership to Coordinator nodes |
| `orchestrix.job.{jobId}.status` | Queue | Job-specific status updates |
| `orchestrix.job.{jobId}.logs` | Queue | Job-specific log entries |
| `orchestrix.job.handoff` | Queue (Competing) | Handoff jobs during scale down |
| `orchestrix.job.handoff.ack.{nodeId}` | Topic | ACK handoff completion |
| `orchestrix.worker.heartbeat` | Topic (Broadcast) | Worker heartbeats |
| `orchestrix.coordinator.heartbeat` | Topic (Broadcast) | Coordinator node heartbeats |

### 3.2 Follower Coordination

When a job is dispatched, the Leader publishes a `JobAssigned` message. Followers compete to claim the job. The winning node subscribes to job-specific channels and handles all events for that job.

```
Leader dispatch job → Publish to job.assigned
       │
       ▼
Followers compete (Consumer Group)
       │
       ▼
Winner subscribes to:
  • job.{jobId}.status
  • job.{jobId}.logs
       │
       ▼
Worker publishes to job-specific channels
       │
       ▼
Winner processes all events → Update DB
```

### 3.3 Scale Down / Job Handoff

When a Coordinator node shuts down (SIGTERM) or crashes:

**Graceful Shutdown:**
1. Node enters DRAINING state
2. Publish owned jobs to `job.handoff`
3. Other nodes compete to take ownership
4. Wait for ACKs (5s timeout, 3 retries)
5. Shutdown

**Crash Recovery:**
1. Coordinator nodes detect missing heartbeat (30s)
2. Leader queries orphaned jobs
3. Re-publish to `job.handoff`
4. Remaining nodes compete and take over

---

## 4. Job Lifecycle

```
                              ┌──────────────────────────────────────────────────────────────────┐
                              │                                                                  │
                              │                         CANCEL (anytime)                         │
                              │                              ▼                                   │
┌─────────┐     ┌───────────┐ │   ┌────────────┐     ┌─────────┐     ┌───────────┐              │
│ Created │ ──► │  Pending  │ ├─► │ Dispatched │ ──► │ Running │ ──► │ Completed │              │
└─────────┘     └─────┬─────┘ │   └────────────┘     └────┬────┘     └───────────┘              │
                      │       │                          │                                      │
                      │       │                          │ ◄──────── Retry ◄─────────┐          │
                      │       │                          │                           │          │
                      ▼       │                          ├─────────────────────┐     │          │
               ┌───────────┐  │                          │                     │     │          │
               │ Scheduled │──┘                          ▼                     ▼     │          │
               │  (Cron/   │                       ┌──────────┐          ┌──────────┐│          │
               │ Interval) │                       │  Failed  │          │ TimedOut ││          │
               └───────────┘                       └────┬─────┘          └────┬─────┘│          │
                                                        │                     │      │          │
                                                        └─────────────────────┴──────┘          │
                                                                  │                              │
                                                                  ▼                              │
                                                            ┌───────────┐                       │
                                                            │ Cancelled │ ◄─────────────────────┘
                                                            └───────────┘
```

### Status Transitions

| From | To | Trigger |
|:-----|:---|:--------|
| Created | Pending | Job enqueued |
| Pending | Dispatched | Leader dispatches (immediate job) |
| Pending | Scheduled | Schedule evaluated (cron/interval) |
| Scheduled | Dispatched | Schedule time reached |
| Dispatched | Running | Worker picks up job |
| Running | Completed | Execution success |
| Running | Failed | Execution error (may retry) |
| Running | TimedOut | Exceeded timeout |
| Failed | Dispatched | Retry triggered |
| TimedOut | Dispatched | Retry triggered |
| Any | Cancelled | Cancel request |

| Status | Description |
|:-------|:------------|
| **Pending** | Job created, waiting to be scheduled |
| **Scheduled** | Schedule evaluated, waiting for dispatch time |
| **Dispatched** | Sent to worker queue |
| **Running** | Worker picked up and executing |
| **Completed** | Execution finished successfully |
| **Failed** | Execution failed (may retry) |
| **TimedOut** | Exceeded timeout threshold |
| **Cancelled** | Cancelled by user/system |

---

## 5. Package Structure

```
Orchestrix/
├── src/
│   ├── Orchestrix.Abstractions/          # Core types, enums, interfaces
│   ├── Transport/
│   │   ├── Orchestrix.Transport.Abstractions/
│   │   ├── Orchestrix.Transport.Redis/
│   │   ├── Orchestrix.Transport.RabbitMQ/
│   │   └── Orchestrix.Transport.Kafka/
│   ├── Locking/
│   │   ├── Orchestrix.Locking.Abstractions/
│   │   ├── Orchestrix.Locking.Redis/
│   │   └── Orchestrix.Locking.InMemory/
│   ├── Coordinator/
│   │   ├── Orchestrix.Coordinator.Abstractions/
│   │   ├── Orchestrix.Coordinator.Persistence.Abstractions/
│   │   └── Orchestrix.Coordinator/
│   ├── Worker/
│   │   └── Orchestrix.Worker/
│   ├── Persistence/
│   │   ├── Orchestrix.Persistence.EfCore/
│   │   └── Orchestrix.Persistence.InMemory/
│   ├── ControlPanel/
│   │   └── Orchestrix.ControlPanel/
│   ├── Api/
│   │   ├── Orchestrix.ServiceApi/
│   │   └── Orchestrix.Admin/
│   └── Client/
│       └── Orchestrix.Client/
├── samples/
│   ├── Orchestrix.Sample.Jobs/
│   ├── Orchestrix.Sample.Coordinator/
│   ├── Orchestrix.Sample.Worker/
│   └── Orchestrix.Sample.Api/
├── tests/
│   ├── Orchestrix.Tests.Unit/
│   └── Orchestrix.Tests.Integration/
└── docs/
    ├── architecture-coordinator-cluster.MD
    └── coordinator-implementation-todo.MD
```

---

## 6. Job Types

### 6.1 Immediate Job
```csharp
await client.EnqueueAsync<SendEmailJob>(new { To = "user@example.com" });
```

### 6.2 Delayed Job
```csharp
await client.ScheduleAsync<SendReminderJob>(
    new { UserId = 123 },
    delay: TimeSpan.FromHours(24)
);
```

### 6.3 Cron Schedule
```csharp
await client.RecurringAsync<DailyReportJob>(
    "daily-report",
    "0 9 * * *" // 9 AM daily
);
```

### 6.4 Interval Schedule
```csharp
await client.RecurringAsync<HealthCheckJob>(
    "health-check",
    TimeSpan.FromMinutes(5)
);
```

---

## 7. Configuration

### Coordinator Configuration
```csharp
services.AddOrchestrixCoordinator(options =>
{
    options.NodeId = "coordinator-1";
    options.DispatchInterval = TimeSpan.FromSeconds(1);
    options.DispatchBatchSize = 100;
    
    options.Cluster.HeartbeatInterval = TimeSpan.FromSeconds(10);
    options.Cluster.DeadNodeTimeout = TimeSpan.FromSeconds(30);
    options.Cluster.HandoffAckTimeout = TimeSpan.FromSeconds(5);
    options.Cluster.HandoffMaxRetries = 3;
});
```

### Worker Configuration
```csharp
services.AddOrchestrixWorker(options =>
{
    options.WorkerName = "worker-1";
    options.Queues = ["video", "email", "default"];
    options.MaxConcurrency = 10;
    options.HeartbeatInterval = TimeSpan.FromSeconds(10);
});
```

---

## 8. Key Design Decisions

| Decision | Rationale |
|:---------|:----------|
| **Job-specific channels** | Single Coordinator node owns all events for a job → full context |
| **Hybrid ACK for handoff** | Balance between reliability and performance |
| **Leader handles scheduling only** | Separation of concerns, followers handle load |
| **Transport abstraction** | Swap message brokers without code changes |
| **Distributed lock for leader** | Simple, proven pattern for leader election |

---

## 9. Observability

- **Metrics**: Job throughput, queue depths, processing times
- **Tracing**: Distributed tracing with correlation IDs
- **Health Checks**: Coordinator, Worker, Transport, Persistence status
- **Dashboard**: Real-time job monitoring and management

---

## 10. References

- [Architecture: Coordinator Cluster](./docs/architecture-coordinator-cluster.MD)
- [Coordinator Implementation TODO](./docs/coordinator-implementation-todo.MD)
