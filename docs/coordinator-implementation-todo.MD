# Orchestrix Coordinator - Implementation TODO

> Detailed checklist for Coordinator implementation.
> Reference: [Architecture](./architecture-coordinator-cluster.MD)

---

## Phase 1: Persistence Abstractions

### 1.1 Entities
- [ ] `JobEntity` - Job record
- [ ] `JobHistoryEntity` - Execution history
- [ ] `CronScheduleEntity` - Cron schedules
- [ ] `IntervalScheduleEntity` - Interval schedules
- [ ] `WorkerEntity` - Worker info
- [ ] `CoordinatorNodeEntity` - Coordinator node info
- [ ] `LogEntry` - Log entries

### 1.2 Stores
- [ ] `IJobStore` / `IJobHistoryStore`
- [ ] `ICronScheduleStore` / `IIntervalScheduleStore`
- [ ] `IWorkerStore` / `ICoordinatorNodeStore`
- [ ] `ILogStore`

---

## Phase 2: Core Services

### 2.1 Main Service
- [ ] `ICoordinatorService` / `CoordinatorService : IHostedService`
- [ ] `CoordinatorOptions`, `CoordinatorClusterOptions`
- [ ] `ServiceCollectionExtensions`

### 2.2 LeaderElection/
- [ ] `ILeaderElection` / `LeaderElection`
- [ ] `LeaderElectionOptions`

### 2.3 Coordination/
- [ ] `ICoordinatorCoordinator` / `CoordinatorCoordinator`
- [ ] `CoordinatorCoordinatorOptions`

---

## Phase 3: Scheduling/
- [ ] `IScheduler` interface
- [ ] `CronExpressionParser` (Cronos)
- [ ] `ScheduleEvaluator`
- [ ] `ScheduleScanner : BackgroundService`
- [ ] `JobPlanner`

---

## Phase 4: Dispatching/
- [ ] `IJobDispatcher` / `JobDispatcher`
- [ ] Publish to `job.dispatch.{queue}`
- [ ] Publish `JobAssignedMessage` to `job.assigned`

---

## Phase 5: RateLimiting/
- [ ] `IRateLimiter` interface
- [ ] `RateLimitOptions`
- [ ] `SlidingWindowRateLimiter`

---

## Phase 6: Handlers/
- [ ] `JobEnqueueHandler` - Subscribe `job.enqueue`
- [ ] `WorkerHeartbeatHandler` - Subscribe `worker.heartbeat`
- [ ] `JobTimeoutMonitor : BackgroundService`

---

## Phase 7: Ownership/ (Follower Coordination)

### 7.1 Core
- [ ] `IJobOwnershipRegistry` / `JobOwnershipRegistry`
- [ ] `JobAssignedMessage` record

### 7.2 Services
- [ ] `JobAssignmentPublisher`
- [ ] `JobAssignmentSubscriber : BackgroundService`
  - [ ] Subscribe `job.assigned` with Consumer Group
  - [ ] Register job ownership
  - [ ] Subscribe to `job.{jobId}.status/logs`
- [ ] `JobEventProcessor` - Process status/logs, update DB
- [ ] `JobOwnershipCleanup` - Cleanup on job complete

---

## Phase 8: Clustering/ (Scale Down & Handoff)

### 8.1 Coordinator Health
- [ ] `CoordinatorNodeInfo` model
- [ ] `ICoordinatorNodeRegistry` / `CoordinatorNodeRegistry`
- [ ] `CoordinatorHeartbeatService : BackgroundService`
- [ ] `CoordinatorHealthMonitor : BackgroundService`

### 8.2 Graceful Shutdown
- [ ] `CoordinatorDrainService` - Handle SIGTERM
  - [ ] Set DRAINING state
  - [ ] Unsubscribe from `job.assigned`
  - [ ] Trigger handoff
  - [ ] Wait for ACKs

### 8.3 Job Handoff (Hybrid ACK)
- [ ] `JobHandoffMessage` / `JobHandoffAckMessage`
- [ ] `JobHandoffPublisher` - Publish, wait ACK, retry
- [ ] `JobHandoffSubscriber : BackgroundService`
- [ ] `JobHandoffAckListener`

### 8.4 Job Ownership Store
- [ ] `IJobOwnershipStore` interface
- [ ] `RedisJobOwnershipStore`
- [ ] `DatabaseJobOwnershipStore`

### 8.5 Crash Recovery
- [ ] `OrphanJobDetector : BackgroundService` (Leader only)

---

## Phase 9: Unit Tests
- [ ] Scheduler tests
- [ ] Dispatcher tests
- [ ] JobOwnershipRegistry tests
- [ ] JobHandoff tests
- [ ] CoordinatorHealthMonitor tests

---

## Folder Structure

```
Orchestrix.Coordinator/
├── Coordination/
├── LeaderElection/
├── Scheduling/
├── Dispatching/
├── RateLimiting/
├── Handlers/
├── Ownership/
├── Clustering/
├── CoordinatorService.cs
├── CoordinatorOptions.cs
├── CoordinatorClusterOptions.cs
└── ServiceCollectionExtensions.cs
```
