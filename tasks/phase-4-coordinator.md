# Phase 4: Coordinator Core

> Central coordinator for job scheduling, dispatching, and cluster coordination.
> See also: [`docs/coordinator-implementation-todo.MD`](../docs/coordinator-implementation-todo.MD)
> 
> **Note**: In single-node mode, the Coordinator acts as both **Leader** AND **Follower**.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Coordinator.Abstractions` | netstandard2.1 | Orchestrix.Abstractions |
| `Orchestrix.Coordinator.Persistence.Abstractions` | netstandard2.1 | Orchestrix.Abstractions |
| `Orchestrix.Coordinator` | netstandard2.1 | Coordinator.Abstractions, Persistence.Abstractions, Transport.Abstractions, Locking.Abstractions |

---

## Folder Structure

### Coordinator.Abstractions
```
src/Coordinator/Orchestrix.Coordinator.Abstractions/
├── Orchestrix.Coordinator.Abstractions.csproj
└── Orchestrix/
    └── Coordinator/
        ├── CoordinatorOptions.cs
        └── ICoordinatorBuilder.cs
```

### Coordinator.Persistence.Abstractions
```
src/Coordinator/Orchestrix.Coordinator.Persistence.Abstractions/
├── Orchestrix.Coordinator.Persistence.Abstractions.csproj
└── Orchestrix/
    └── Coordinator/
        └── Persistence/
            ├── Entities/
            │   ├── JobEntity.cs
            │   ├── JobHistoryEntity.cs
            │   ├── CronScheduleEntity.cs
            │   ├── IntervalScheduleEntity.cs
            │   ├── WorkerEntity.cs
            │   ├── CoordinatorNodeEntity.cs
            │   ├── LogEntry.cs
            │   └── DeadLetterEntity.cs
            ├── IJobStore.cs
            ├── IJobHistoryStore.cs
            ├── ICronScheduleStore.cs
            ├── IIntervalScheduleStore.cs
            ├── IWorkerStore.cs
            ├── ICoordinatorNodeStore.cs
            ├── ILogStore.cs
            └── IDeadLetterStore.cs
```

### Coordinator (Main Implementation)
```
src/Coordinator/Orchestrix.Coordinator/
├── Orchestrix.Coordinator.csproj
└── Orchestrix/
    └── Coordinator/
        ├── CoordinatorBuilder.cs
        ├── CoordinatorClusterOptions.cs
        ├── ICoordinatorService.cs
        ├── CoordinatorService.cs
        ├── ServiceCollectionExtensions.cs
        ├── Caching/
        │   ├── CacheKeys.cs
        │   ├── CacheOptions.cs
        │   ├── ICacheService.cs
        │   └── CacheService.cs
        ├── LeaderElection/
        │   ├── ILeaderElection.cs
        │   ├── LeaderElection.cs
        │   └── LeaderElectionOptions.cs
        ├── Coordination/
        │   ├── ICoordinatorCoordinator.cs
        │   ├── CoordinatorCoordinator.cs
        │   └── CoordinatorCoordinatorOptions.cs
        ├── Scheduling/
        │   ├── IScheduler.cs
        │   ├── CronExpressionParser.cs
        │   ├── ScheduleEvaluator.cs
        │   ├── ScheduleScanner.cs
        │   └── JobPlanner.cs
        ├── Dispatching/
        │   ├── IJobDispatcher.cs
        │   └── JobDispatcher.cs
        ├── RateLimiting/
        │   ├── IRateLimiter.cs
        │   ├── RateLimitOptions.cs
        │   └── SlidingWindowRateLimiter.cs
        ├── Handlers/
        │   ├── JobEnqueueHandler.cs
        │   ├── WorkerHeartbeatHandler.cs
        │   └── JobTimeoutMonitor.cs
        ├── Ownership/
        │   ├── IJobOwnershipRegistry.cs
        │   ├── JobOwnershipRegistry.cs
        │   ├── JobLoadInfo.cs
        │   ├── JobAssignmentPublisher.cs
        │   ├── JobAssignmentSubscriber.cs
        │   ├── JobEventProcessor.cs
        │   ├── JobOwnershipCleanup.cs
        │   └── JobLoadBalancer.cs
        ├── Clustering/
        │   ├── CoordinatorNodeInfo.cs
        │   ├── ICoordinatorNodeRegistry.cs
        │   ├── CoordinatorNodeRegistry.cs
        │   ├── CoordinatorHeartbeatService.cs
        │   ├── CoordinatorHealthMonitor.cs
        │   ├── CoordinatorDrainService.cs
        │   ├── JobHandoffPublisher.cs
        │   ├── JobHandoffSubscriber.cs
        │   ├── JobHandoffAckListener.cs
        │   └── OrphanJobDetector.cs
        └── ChannelCleanup/
            ├── ChannelCleanupScanner.cs
            └── ChannelCleanupOptions.cs
```

**Namespaces:**
- `Orchestrix.Coordinator` - Core coordinator abstractions and services
- `Orchestrix.Coordinator.Persistence` - Persistence abstractions (entities & stores)
- `Orchestrix.Coordinator.Caching` - Caching services
- `Orchestrix.Coordinator.LeaderElection` - Leader election logic
- `Orchestrix.Coordinator.Coordination` - Coordinator-to-coordinator communication
- `Orchestrix.Coordinator.Scheduling` - Job scheduling
- `Orchestrix.Coordinator.Dispatching` - Job dispatching
- `Orchestrix.Coordinator.RateLimiting` - Rate limiting
- `Orchestrix.Coordinator.Handlers` - Event handlers
- `Orchestrix.Coordinator.Ownership` - Job ownership & follower coordination
- `Orchestrix.Coordinator.Clustering` - Cluster management & scale down
- `Orchestrix.Coordinator.ChannelCleanup` - Channel cleanup logic

---

## 4.0 Coordinator.Abstractions

> Options and Builder interface only. No registration logic here.

> **Path:** `src/Coordinator/Orchestrix.Coordinator.Abstractions/`

### Files
- [ ] `CoordinatorOptions.cs`
  ```csharp
  public class CoordinatorOptions
  {
      public string NodeId { get; set; } = Guid.NewGuid().ToString();
      public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(5);
      public TimeSpan LeaderLeaseDuration { get; set; } = TimeSpan.FromSeconds(30);
      public TimeSpan LeaderRenewInterval { get; set; } = TimeSpan.FromSeconds(10);
  }
  ```

- [ ] `ICoordinatorBuilder.cs`
  ```csharp
  public interface ICoordinatorBuilder
  {
      IServiceCollection Services { get; }
  }
  ```

**Files: 2**

---

## 4.1 Coordinator.Persistence.Abstractions

> **Path:** `src/Coordinator/Orchestrix.Coordinator.Persistence.Abstractions/`



### Entities (in subfolder `Entities/`)
- [ ] `JobEntity.cs`
  ```csharp
  public class JobEntity
  {
      public Guid Id { get; set; }
      public required string JobType { get; set; }
      public required string Queue { get; set; }
      public JobStatus Status { get; set; }
      public JobPriority Priority { get; set; }
      public string? ArgumentsJson { get; set; }
      public string? FollowerNodeId { get; set; }
      public string? WorkerId { get; set; }
      public int RetryCount { get; set; }
      public int MaxRetries { get; set; }
      public string? RetryPolicyJson { get; set; }
      public string? Error { get; set; }
      public string? CorrelationId { get; set; }
      public DateTimeOffset CreatedAt { get; set; }
      public DateTimeOffset? ScheduledAt { get; set; }
      public DateTimeOffset? DispatchedAt { get; set; }
      public DateTimeOffset? StartedAt { get; set; }
      public DateTimeOffset? CompletedAt { get; set; }
      public TimeSpan? Timeout { get; set; }
      public bool ChannelsCleaned { get; set; } = false; // For cleanup tracking
  }
  ```
- [ ] `JobHistoryEntity.cs`
- [ ] `CronScheduleEntity.cs`
- [ ] `IntervalScheduleEntity.cs`
- [ ] `WorkerEntity.cs`
- [ ] `CoordinatorNodeEntity.cs`
- [ ] `LogEntry.cs`
- [ ] `DeadLetterEntity.cs` - Jobs failed after max retries

### Store Interfaces
- [ ] `IJobStore.cs`
  ```csharp
  public interface IJobStore
  {
      Task<JobEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
      Task<JobEntity> CreateAsync(JobEntity entity, CancellationToken ct = default);
      Task UpdateAsync(JobEntity entity, CancellationToken ct = default);
      
      // Follower queries
      Task<IReadOnlyList<JobEntity>> GetByFollowerAsync(string followerNodeId, CancellationToken ct = default);
      Task<int> GetCountByFollowerAsync(string followerNodeId, CancellationToken ct = default);
      Task ClearFollowerAsync(string followerNodeId, CancellationToken ct = default);
      
      // Status queries
      Task<IReadOnlyList<JobEntity>> GetPendingAsync(int limit, CancellationToken ct = default);
      Task<IReadOnlyList<JobEntity>> GetScheduledAsync(DateTimeOffset beforeTime, int limit, CancellationToken ct = default);
      
      // Cleanup queries
      Task<IReadOnlyList<JobEntity>> GetForCleanupAsync(
          DateTimeOffset completedBefore, 
          int limit, 
          CancellationToken ct = default);
      Task MarkChannelsCleanedAsync(Guid jobId, CancellationToken ct = default);
  }
  ```
- [ ] `IJobHistoryStore.cs`
- [ ] `ICronScheduleStore.cs`
- [ ] `IIntervalScheduleStore.cs`
- [ ] `IWorkerStore.cs`
- [ ] `ICoordinatorNodeStore.cs`
- [ ] `ILogStore.cs`
- [ ] `IDeadLetterStore.cs`

**Files: 16**

---

## 4.2 Coordinator - Core

### Usage Example
```csharp
// Registration (in Orchestrix.Coordinator)
services.AddOrchestrixCoordinator(options =>
{
    options.NodeId = "coordinator-1";
})
.UseRedisTransport(redis => redis.ConnectionString = "localhost:6379")  // from Transport.Redis
.UseEfCorePersistence(ef => ef.UseSqlServer("..."))                     // from Persistence.EfCore  
.UseRedisLocking(redis => redis.ConnectionString = "localhost:6379");   // from Locking.Redis
```

### Files
- [ ] `CoordinatorBuilder.cs` - Implementation of `ICoordinatorBuilder`
- [ ] `CoordinatorClusterOptions.cs`
- [ ] `ICoordinatorService.cs` / `CoordinatorService.cs`
- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static ICoordinatorBuilder AddOrchestrixCoordinator(
      this IServiceCollection services,
      Action<CoordinatorOptions>? configure = null)
  {
      var options = new CoordinatorOptions();
      configure?.Invoke(options);
      services.AddSingleton(options);
      // Register core services...
      return new CoordinatorBuilder(services);
  }
  ```

**Files: 4**

---

## 4.3 Coordinator - Caching/

> Use `IDistributedCache` (.NET built-in) to cache hot data.

### Cache Keys
```csharp
public static class CacheKeys
{
    public static string Job(Guid jobId) => $"orchestrix:job:{jobId}";
    public static string JobsByFollower(string nodeId) => $"orchestrix:jobs:follower:{nodeId}";
    public static string Worker(string workerId) => $"orchestrix:worker:{workerId}";
    public static string Workers() => "orchestrix:workers";
    public static string CoordinatorNode(string nodeId) => $"orchestrix:coordinator:{nodeId}";
    public static string CoordinatorNodes() => "orchestrix:coordinators";
}
```

### Files
- [ ] `CacheKeys.cs` - Cache key constants
- [ ] `CacheOptions.cs`
  ```csharp
  public class CacheOptions
  {
      public TimeSpan JobCacheTtl { get; set; } = TimeSpan.FromMinutes(5);
      public TimeSpan WorkerCacheTtl { get; set; } = TimeSpan.FromSeconds(30);
      public TimeSpan NodeCacheTtl { get; set; } = TimeSpan.FromSeconds(30);
  }
  ```
- [ ] `ICacheService.cs` / `CacheService.cs` - Wrapper over IDistributedCache

### Local In-Memory Cache (per Follower)
```csharp
// Each Follower tracks owned jobs in local memory (not via Redis)
ConcurrentDictionary<Guid, JobInfo> _ownedJobs;
// Sync with DB on startup, update on claim/release
```

**Files: 3**

---

## 4.4 Coordinator - LeaderElection/

- [ ] `ILeaderElection.cs`
- [ ] `LeaderElection.cs`
- [ ] `LeaderElectionOptions.cs`

---

## 4.5 Coordinator - Coordination/

- [ ] `ICoordinatorCoordinator.cs`
- [ ] `CoordinatorCoordinator.cs`
- [ ] `CoordinatorCoordinatorOptions.cs`

---

## 4.6 Coordinator - Scheduling/

- [ ] `IScheduler.cs`
- [ ] `CronExpressionParser.cs` (uses Cronos)
- [ ] `ScheduleEvaluator.cs`
- [ ] `ScheduleScanner.cs` : BackgroundService
- [ ] `JobPlanner.cs`

---

## 4.7 Coordinator - Dispatching/

- [ ] `IJobDispatcher.cs`
- [ ] `JobDispatcher.cs`

---

## 4.8 Coordinator - RateLimiting/

- [ ] `IRateLimiter.cs`
- [ ] `RateLimitOptions.cs`
- [ ] `SlidingWindowRateLimiter.cs`

---

## 4.9 Coordinator - Handlers/

- [ ] `JobEnqueueHandler.cs`
- [ ] `WorkerHeartbeatHandler.cs`
- [ ] `JobTimeoutMonitor.cs` : BackgroundService

---

## 4.10 Coordinator - Ownership/ (Follower Coordination)

### Load Balancing Strategy
Ensure jobs are distributed evenly across Follower nodes:

```
┌─────────────────────────────────────────────────────────────────────┐
│                    FOLLOWER LOAD BALANCING                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  1. Consumer Group (Built-in)                                       │
│     • Redis Streams Consumer Group auto round-robin                 │
│     • Each job.assigned message goes to only 1 follower             │
│                                                                     │
│  2. Load Tracking                                                   │
│     • Each node tracks number of owned jobs                         │
│     • Publish load info in heartbeat                                │
│                                                                     │
│  3. Rebalancing (Optional)                                          │
│     • If imbalance detected (node A: 100 jobs, node B: 10 jobs)     │
│     • Overloaded node can handoff some jobs                         │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Files
- [ ] `IJobOwnershipRegistry.cs`
- [ ] `JobOwnershipRegistry.cs` - Track owned jobs per node
- [ ] `JobLoadInfo.cs` - Load info (jobCount, lastUpdated)
- [ ] `JobAssignmentPublisher.cs` - Uses `JobAssignedMessage` from Transport
- [ ] `JobAssignmentSubscriber.cs` : BackgroundService (uses Consumer Group)
- [ ] `JobEventProcessor.cs`
- [ ] `JobOwnershipCleanup.cs`
- [ ] `JobLoadBalancer.cs` - Monitor load, trigger rebalance if needed

---

## 4.11 Coordinator - Clustering/ (Scale Down & Handoff)

- [ ] `CoordinatorNodeInfo.cs`
- [ ] `ICoordinatorNodeRegistry.cs` / `CoordinatorNodeRegistry.cs`
- [ ] `CoordinatorHeartbeatService.cs` : BackgroundService
- [ ] `CoordinatorHealthMonitor.cs` : BackgroundService
- [ ] `CoordinatorDrainService.cs`
- [ ] `JobHandoffPublisher.cs` - Uses `JobHandoffMessage` from Transport
- [ ] `JobHandoffSubscriber.cs` : BackgroundService
- [ ] `JobHandoffAckListener.cs` - Uses `JobHandoffAckMessage` from Transport
- [ ] `OrphanJobDetector.cs` : BackgroundService

---

## 4.12 Coordinator - ChannelCleanup/

> When job completes (success/fail), **Leader (Master)** scans DB to cleanup channels.
> **Only Leader runs cleanup** to avoid race condition between nodes.
> Cleanup status is tracked in DB (`ChannelsCleaned` flag in `JobEntity`).

### Flow
```
Job Complete/Failed
       ↓
   [Follower] Update job status → Completed/Failed
       ↓
   [Leader] ChannelCleanupScanner (BackgroundService)
       ↓
   Scan DB: SELECT jobs WHERE 
     Status IN (Completed, Failed) 
     AND ChannelsCleaned = false
     AND CompletedAt < NOW() - CleanupDelay
       ↓
   For each job:
     1. CloseChannelAsync(Job.Status(jobId))
     2. CloseChannelAsync(Job.Logs(jobId))
     3. UPDATE job SET ChannelsCleaned = true
```

### JobEntity Extension
```csharp
public class JobEntity
{
    // ... existing fields ...
    public bool ChannelsCleaned { get; set; } = false;
}
```

### Files
- [ ] `ChannelCleanupScanner.cs` : BackgroundService (Leader only)
  ```csharp
  // Scan DB periodically (e.g., every 10s)
  // Find completed jobs where ChannelsCleaned = false
  // AND CompletedAt + CleanupDelay < now
  // Cleanup and mark ChannelsCleaned = true
  ```
- [ ] `ChannelCleanupOptions.cs`
  ```csharp
  public class ChannelCleanupOptions
  {
      public TimeSpan CleanupDelay { get; set; } = TimeSpan.FromSeconds(30);
      public TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(10);
      public int BatchSize { get; set; } = 100;
  }
  ```

---

## Unit Tests

- [ ] `LeaderElectionTests.cs`
  - Election process
  - Failover when leader dies
  - Lock renewal
- [ ] `JobDispatcherTests.cs`
  - Dispatch to correct queue
  - Retry handling
- [ ] `JobStateMachineTests.cs`
  - State transitions: Pending → Running → Completed/Failed
- [ ] `ScheduleScannerTests.cs`
  - Cron parsing and next run calculation
  - Interval schedules
- [ ] `WorkerRegistryTests.cs`
  - Worker registration and heartbeat
- [ ] `RateLimiterTests.cs`
  - Sliding window logic

---

## Summary
| Section | Files |
|:--------|:------|
| Persistence.Abstractions | 16 |
| Core | 4 |
| Caching | 3 |
| LeaderElection | 3 |
| Coordination | 3 |
| Scheduling | 5 |
| Dispatching | 2 |
| RateLimiting | 3 |
| Handlers | 3 |
| Ownership | 8 |
| Clustering | 8 |
| ChannelCleanup | 2 |
| Unit Tests | 6 |
| **Total** | **~66** |

---

## Design Notes

### Transport Abstraction
Coordinator **does not depend directly on Redis**. All communication goes through:
- `IPublisher` / `ISubscriber` (Transport.Abstractions)
- `IDistributedLock` (Locking.Abstractions)
- `IJobStore` (Coordinator.Persistence.Abstractions) - includes ownership queries

### Consumer Group Support
Transport abstraction needs to support competing consumers and channel cleanup:
```csharp
public interface ISubscriber
{
    // Broadcast - all subscribers receive
    Task SubscribeAsync<T>(string channel, Func<T, Task> handler, CancellationToken ct);
    
    // Competing consumers (only one receives)
    Task SubscribeCompetingAsync<T>(
        string channel, 
        string groupName, 
        string consumerName,
        Func<T, Task> handler,
        CancellationToken ct);
    
    Task UnsubscribeAsync(string channel);
    
    // Close channel and cleanup resources
    Task CloseChannelAsync(string channel, CancellationToken ct);
}
```

### Minimum Requirement: Redis
- Redis is the default and recommended implementation
- Other transports (RabbitMQ, Kafka) can be implemented later
- InMemory is for testing only
