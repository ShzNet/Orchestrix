# Phase 4: Coordinator - Implementation Stages

> **Coordinator** is the heart of Orchestrix - manages scheduling, dispatching, and cluster coordination.
> 
> **Architecture**: Hybrid Active-Active Cluster
> - **Leader** (1 node): Scheduling, Dispatching, Timeout Monitoring
> - **Follower** (all nodes): Event Processing, Load Balanced

---

## Components Overview

This phase implements 12 major components that work together to provide distributed job coordination:

### 1. Persistence Abstractions
**Purpose**: Define storage contracts for all coordinator data  
**What**: 8 entities (Job, Schedule, Worker, etc.) + 8 store interfaces  
**Why**: Decouple coordinator logic from specific database implementations  
**How**: Create abstract entities and repository interfaces that can be implemented with any storage backend (EF Core, Dapper, etc.)

### 2. Communication Abstractions
**Purpose**: Define channels and messages for coordinator cluster communication  
**What**: Channel constants and message classes for Coordinator-to-Coordinator communication  
**Why**: Provide clear contracts for coordinator nodes to communicate. Admin/Control Panel communication will be designed later  
**How**: Cluster Communication - Messages for coordinator coordination (metrics, job handoff)

### 3. Core Services
**Purpose**: Provide foundation for coordinator functionality  
**What**: Options classes, builder pattern, main service orchestration  
**Why**: Enable flexible configuration and DI-based architecture  
**How**: Implement `IHostedService` that manages lifecycle of all sub-components

### 4. Caching Layer
**Purpose**: Reduce database load for frequently accessed data  
**What**: Distributed cache wrapper with key generation helpers  
**Why**: Improve performance by caching hot data (jobs, workers, nodes)  
**How**: Wrap `IDistributedCache` with typed methods and automatic serialization

### 5. Leader Election
**Purpose**: Ensure only one coordinator performs scheduling/monitoring  
**What**: Distributed lock-based election with automatic failover  
**Why**: Prevent duplicate job creation and race conditions  
**How**: Use distributed lock with TTL, background renewal, automatic re-election on failure

### 6. Scheduling
**Purpose**: Automatically create jobs from cron schedules  
**What**: Schedule scanner, cron parser, job planner  
**Why**: Support recurring jobs without manual intervention  
**How**: Leader scans DB every 10s for due cron schedules, creates jobs, updates next run time

### 7. Dispatching
**Purpose**: Send jobs to workers for execution  
**What**: Job dispatcher that publishes to transport channels  
**Why**: Decouple job creation from execution  
**How**: Publish to `job.dispatch.{queue}` for workers and `job.assigned` for follower coordination

### 8. Rate Limiting
**Purpose**: Control job dispatch rate per queue  
**What**: Sliding window rate limiter  
**Why**: Prevent overwhelming workers or downstream systems  
**How**: Track timestamps in time window, reject requests exceeding limit

### 9. Event Handlers
**Purpose**: Process incoming events from workers and clients  
**What**: Job enqueue handler, worker heartbeat handler, timeout monitor  
**Why**: React to external events and maintain system state  
**How**: Subscribe to transport channels, process messages, update database

### 10. Follower Coordination (Ownership)
**Purpose**: Distribute job event processing across coordinator nodes  
**What**: Job ownership registry, assignment subscriber, event processor  
**Why**: Scale event processing horizontally, ensure single source of truth per job  
**How**: Use consumer groups for job assignment, each follower owns subset of jobs and subscribes to job-specific channels

### 11. Clustering & Scale Down
**Purpose**: Handle coordinator node failures and graceful shutdown  
**What**: Heartbeat service, health monitor, drain service, handoff mechanism, orphan detector  
**Why**: Ensure zero job loss during node failures or deployments  
**How**: 
- **Graceful**: Node draining hands off jobs to other nodes with ACK protocol
- **Crash**: Leader detects dead nodes via heartbeat timeout, reassigns orphaned jobs

### 12. Channel Cleanup
**Purpose**: Remove job-specific channels after completion  
**What**: Background scanner that closes channels  
**Why**: Prevent channel/memory leaks in transport layer  
**How**: Leader scans completed jobs, closes `job.{id}.status` and `job.{id}.logs` channels after delay

### 13. Coordinator Communication
**Purpose**: Enable direct messaging between coordinator nodes  
**What**: Point-to-point and broadcast messaging  
**Why**: Support advanced features like load rebalancing  
**How**: Publish to `coordinator.{nodeId}.messages` or `coordinator.broadcast` channels

---

## Projects Structure

```
src/Coordinator/
├── Orchestrix.Coordinator.Abstractions/
├── Orchestrix.Coordinator.Persistence.Abstractions/
└── Orchestrix.Coordinator/
```

**Total**: ~62 files across 12 stages

---

## Stage 1: Persistence Abstractions ✅

> **Status**: ✅ **COMPLETE**
> 
> **Goal**: Define all storage entities and interfaces
> 
> **Project**: `Orchestrix.Coordinator.Persistence.Abstractions`
> 
> **Dependencies**: None (self-contained with PolySharp for `required` keyword)
> 
> **Files**: 17 (8 entities + 8 store interfaces + 1 enum)

### 1.1 Project Setup ✅

- [x] Create project targeting `netstandard2.1`
- [x] Add PolySharp package for `required` keyword support
- [x] Create folder structure: `Orchestrix/Coordinator/Persistence/Entities/`
- [x] Add to solution under `/src/Coordinator/`
- [x] Create `JobStatus` enum

### 1.2 Entities Implementation ✅

**Location**: `Entities/` subfolder

- [x] **JobEntity.cs** - Core job record ✅
- [x] **JobHistoryEntity.cs** - Execution history ✅
- [x] **CronScheduleEntity.cs** - Cron-based recurring schedules ✅
- [x] **IntervalScheduleEntity.cs** - Interval-based recurring schedules ✅
- [x] **WorkerEntity.cs** - Worker registration and health tracking ✅
- [x] **CoordinatorNodeEntity.cs** - Coordinator cluster node tracking ✅
- [x] **LogEntry.cs** - Job execution logs ✅
- [x] **DeadLetterEntity.cs** - Failed jobs after max retries ✅

### 1.3 Store Interfaces Implementation ✅

**All interfaces refactored to be use-case driven instead of generic CRUD:**

- [x] **IJobStore.cs** - Job lifecycle & coordination ✅
  - `EnqueueAsync`, `MarkDispatchedAsync`, `UpdateStatusAsync`
  - `GetPendingJobsAsync`, `GetDueScheduledJobsAsync`
  - `TryClaimJobAsync`, `GetJobsByFollowerAsync`, `ReleaseJobsFromDeadFollowerAsync`
  - `TryRetryJobAsync`, `MoveToDeadLetterAsync`
  - `GetJobsNeedingCleanupAsync`, `MarkChannelsCleanedAsync`

- [x] **IJobHistoryStore.cs** - Execution history ✅
  - `CreateAsync`, `GetByJobIdAsync`

- [x] **ICronScheduleStore.cs** - Cron scheduling ✅
  - `RegisterScheduleAsync`, `UpdateNextRunTimeAsync`, `SetEnabledAsync`
  - `GetDueSchedulesAsync`, `RemoveScheduleAsync`, `GetAllSchedulesAsync`

- [x] **IIntervalScheduleStore.cs** - Interval scheduling ✅
  - `RegisterScheduleAsync`, `UpdateNextRunTimeAsync`, `SetEnabledAsync`
  - `GetDueSchedulesAsync`, `RemoveScheduleAsync`, `GetAllSchedulesAsync`

- [x] **IWorkerStore.cs** - Worker lifecycle & capacity ✅
  - `UpdateHeartbeatAsync`, `MarkDrainingAsync`, `RemoveWorkerAsync`
  - `GetAvailableWorkersForQueueAsync`, `GetDeadWorkersAsync`, `GetAllActiveWorkersAsync`

- [x] **ICoordinatorNodeStore.cs** - Cluster coordination ✅
  - `UpdateHeartbeatAsync`, `UpdateRoleAsync`, `IncrementJobCountAsync`, `DecrementJobCountAsync`
  - `MarkDrainingAsync`, `RemoveNodeAsync`
  - `GetDeadNodesAsync`, `GetActiveFollowersAsync`, `GetAllActiveNodesAsync`

- [x] **ILogStore.cs** - Job logs ✅
  - `AppendAsync`, `GetByJobIdAsync`

- [x] **IDeadLetterStore.cs** - Failed job tracking ✅
  - `AddToDeadLetterAsync`, `GetAllDeadLettersAsync`, `GetByIdAsync`, `RemoveAsync`

### 1.4 Verification ✅

- [x] Build project: `dotnet build` ✅
- [x] Verify 0 warnings, 0 errors ✅
- [x] Added to solution ✅

---

## Stage 2: Communication Abstractions ✅

> **Status**: ✅ **COMPLETE**
> 
> **Goal**: Define channels and messages for external communication
> 
> **Project**: `Orchestrix.Coordinator.Abstractions`
> 
> **Folder**: `Orchestrix/Coordinator/Communication/`
> 
> **Files**: ~5 (1 channel helper + 1 enum + 3 messages)
> 
> **Purpose**: Define communication contracts for:
> - Coordinator ↔ Coordinator (cluster coordination)
> - **Note**: Job events are shared across Worker, Coordinator, and Control Panel (already defined in `Orchestrix.Transport.Abstractions`)
> - **Note**: Admin/Control Panel communication will be designed later

### 2.1 Channel Helpers ✅

- [x] **CoordinatorChannels.cs** - Channel name builder (follows TransportChannels pattern) ✅
  - `CoordinatorMetrics` - Node metrics + heartbeat
  - `JobDispatched` - Broadcast to followers
  - `JobHandoff` - Job reassignment (no ACK needed - DB update is confirmation)
- [x] **CoordinatorRole.cs** - Coordinator role enum (Leader/Follower) ✅

**Note**: Job channels (`job.dispatch`, `job.{executionId}.status`, `job.{executionId}.logs`) and Worker channels (`worker.join`, `worker.{id}.metrics`) are already defined in `TransportChannels` and shared across all components.

### 2.2 Coordinator Cluster Messages ✅

- [x] **NodeMetricsMessage.cs** ✅ - Node metrics + heartbeat (combined)
- [x] **JobDispatchedEvent.cs** ✅ - Broadcast to followers for race-to-claim ownership (with ExecutionId)
- [x] **JobHandoffMessage.cs** ✅ - Job reassignment during scale down/crash (with ExecutionId and HandoffReason enum)

### Shared Channels (from Transport.Abstractions)

**Job and Worker channels are already defined in `Orchestrix.Transport.Abstractions`** - no need to redefine here.

**Follower Coordination Pattern (Broadcast + Race to Claim):**
1. Leader dispatches job → broadcasts `JobDispatchedEvent` to `job.dispatched` (Topic)
2. **All followers receive** the event
3. Followers **race to claim ownership** via DB update:
   - `UPDATE Jobs SET FollowerNodeId = @nodeId WHERE Id = @jobId AND FollowerNodeId IS NULL`
4. First follower to succeed becomes owner
5. Owner subscribes to `job.{jobId}.status` and `job.{jobId}.logs`
6. Owner has full context of job lifecycle

### Implementation Notes

- **Configurable prefix** allows multiple Orchestrix instances on same transport
- **Control Panel queries data directly** from storage (via `Persistence.Abstractions`)
- **Channels are for realtime updates only** - no request/reply for queries
- **Job events are shared** - no duplication across components

### Verification

- [ ] Verify channel names use configurable prefix
- [ ] Verify all message classes have required properties
- [ ] Verify messages are serializable (no complex types)
- [ ] Verify job events are reused from `Orchestrix.Abstractions`

---

## Stage 3: Core Services & Options ✅

> **Status**: ✅ **COMPLETE**
> 
> **Goal**: Setup basic coordinator structure and DI registration
> 
> **Projects**: `Orchestrix.Coordinator.Abstractions` + `Orchestrix.Coordinator`
> 
> **Files**: ~7 (2 abstractions + 2 implementations + 3 builder interfaces)

### 3.1 Abstractions Project ✅

- [x] **CoordinatorOptions.cs** - Configuration class with nested builders ✅
- [x] **ICoordinatorBuilder.cs** - Fluent builder interface ✅
- [x] **IPersistenceBuilder.cs** - Persistence configuration interface ✅

### 3.2 Core Implementation ✅

- [x] **CoordinatorBuilder.cs** - Implements ICoordinatorBuilder ✅
- [x] **Builders.cs** - Internal implementations of TransportBuilder, LockingBuilder, PersistenceBuilder ✅
- [x] **ServiceCollectionExtensions.cs** - `AddOrchestrixCoordinator()` method ✅

### 3.3 Builder Interfaces ✅

- [x] **ITransportBuilder** - Added to Transport.Abstractions ✅
- [x] **ILockingBuilder** - Added to Locking.Abstractions ✅

### 3.4 Locking Implementations Updated ✅

- [x] **Orchestrix.Locking.Redis** - UseRedis(ILockingBuilder) with TryAddSingleton ✅
- [x] **Orchestrix.Locking.InMemory** - UseInMemory(ILockingBuilder) ✅
- [x] Removed LockingOptions ✅

### 3.5 Verification ✅

- [x] Build both projects successfully ✅
- [x] Verify DI registration works ✅

---


## Stage 4: Caching Layer ✅

> **Status**: ✅ **COMPLETE**
> 
> **Goal**: Extract caching to reusable library
> 
> **Project**: `Orchestrix.Caching`
> 
> **Files**: 1 (extension methods)

### Implementation ✅

- [x] **Created Orchestrix.Caching library** ✅
- [x] **DistributedCacheExtensions.cs** - Extension methods for IDistributedCache ✅
  - `GetObjectAsync<T>()` - Deserialize from cache
  - `SetObjectAsync<T>()` - Serialize to cache with TTL
  - JSON serialization built-in
- [x] **Added CachePrefix to CoordinatorOptions** ✅
- [x] **Removed CacheService/ICacheService from Coordinator** ✅

### Verification ✅

- [x] Build successful ✅
- [x] Extension methods in correct namespace (Microsoft.Extensions.Caching.Distributed) ✅

---


## Stage 5: Leader Election ✅

> **Status**: ✅ **COMPLETE**
> 
> **Goal**: Implement distributed leader election
> 
> **Folder**: `Orchestrix.Coordinator/LeaderElection/`
> 
> **Files**: 3
> 
> **Complexity**: **HIGH** - Critical for cluster coordination

### Implementation ✅

- [x] **ILeaderElection.cs** ✅
  - Property: `bool IsLeader { get; }`
  - Event: `LeadershipChanged` for reactive services
  - Methods: `StartAsync`, `StopAsync`

- [x] **LeaderElection.cs** - Core election logic ✅
  - Use `IDistributedLockProvider` to acquire lock `orchestrix:coordinator:leader`
  - Election algorithm:
    1. Attempt to acquire lock with TTL = LeaseDuration
    2. If successful → set `IsLeader = true`, raise event
    3. Background task extends lock every RenewInterval via `ExtendAsync`
    4. If extension fails → set `IsLeader = false`, raise event, retry acquisition
  - Handle failover automatically
  - Linked cancellation token for proper shutdown

- [x] **LeaderElectionHostedService.cs** - Lifecycle management ✅
  - Auto start/stop with application

### Verification ✅

- [x] Build successful ✅
- [x] Proper cancellation handling ✅
- [x] Event-driven design ✅

---

## Stage 6: Scheduling

> **Goal**: Scan and evaluate schedules, create jobs
> 
> **Folder**: `Orchestrix.Coordinator/Scheduling/`
> 
> **Files**: 5
> 
> **Complexity**: **HIGH** - Core scheduling logic

### Implementation

- [ ] **IScheduler.cs** - Scheduling interface

- [ ] **CronExpressionParser.cs** - Cron parsing utility
  - Use `Cronos` NuGet package
  - Method: `GetNextOccurrence(cronExpression, from)` → DateTimeOffset

- [ ] **ScheduleEvaluator.cs** - Schedule evaluation logic
  - Determine if schedule is due
  - Handle timezone conversions

- [ ] **ScheduleScanner.cs** - Background service (Leader only)
  - Check `ILeaderElection.IsLeader` before processing
  - Run every 10 seconds
  - Process flow:
    1. Query cron/interval schedules where `NextRunTime <= NOW` AND `IsEnabled = true`
    2. For each due schedule: create JobEntity, dispatch, update NextRunTime
  - Separate methods: `ScanCronSchedulesAsync`, `ScanIntervalSchedulesAsync`

- [ ] **JobPlanner.cs** - Job creation from schedules
  - Method: `PlanJobAsync(schedule)` → create job entity, save to DB, dispatch

### Verification

- [ ] Create cron schedule `"*/1 * * * *"` → verify job created every minute
- [ ] Create interval schedule (30s) → verify job created every 30 seconds
- [ ] Verify timezone handling works correctly

---

## Stage 7: Dispatching

> **Goal**: Dispatch jobs to workers via transport
> 
> **Folder**: `Orchestrix.Coordinator/Dispatching/`
> 
> **Files**: 2

### Implementation

- [ ] **IJobDispatcher.cs**
  - Method: `DispatchAsync(JobEntity job)`

- [ ] **JobDispatcher.cs** - Dispatch implementation
  - Dispatch flow:
    1. Publish `JobDispatchMessage` → `job.dispatch.{queue}` channel (for workers)
    2. Publish `JobAssignedMessage` → `job.assigned` channel (for Follower coordination)
    3. Update job status → `Dispatched`, set `DispatchedAt` timestamp

### Verification

- [ ] Dispatch job → verify messages published to correct channels
- [ ] Verify job status updated in database

---

## Stage 8: Rate Limiting

> **Goal**: Implement sliding window rate limiter
> 
> **Folder**: `Orchestrix.Coordinator/RateLimiting/`
> 
> **Files**: 3

### Implementation

- [ ] **IRateLimiter.cs**
  - Method: `TryAcquireAsync(key, limit, window)` → bool

- [ ] **RateLimitOptions.cs**
  - Default limits configuration per queue

- [ ] **SlidingWindowRateLimiter.cs** - Rate limiting algorithm
  - Implementation approach:
    1. Use `ConcurrentDictionary<string, Queue<DateTimeOffset>>` to track timestamps
    2. On acquire: remove timestamps outside time window
    3. Check if count < limit
    4. If yes: add current timestamp and return true

### Verification

- [ ] Set limit 10 requests/60s → send 10 requests → all succeed
- [ ] Send 11th request → should fail
- [ ] Wait 60s → request should succeed again

---

## Stage 9: Event Handlers

> **Goal**: Handle incoming events from workers and clients
> 
> **Folder**: `Orchestrix.Coordinator/Handlers/`
> 
> **Files**: 3

### Implementation

- [ ] **JobEnqueueHandler.cs** - Background service
  - Subscribe to `job.enqueue` channel
  - Processing flow:
    1. Receive `JobEnqueueMessage`
    2. Create `JobEntity` in database
    3. Dispatch via `IJobDispatcher`

- [ ] **WorkerHeartbeatHandler.cs** - Background service
  - Subscribe to `worker.heartbeat` channel
  - Processing flow:
    1. Receive `WorkerHeartbeatMessage`
    2. Upsert `WorkerEntity` (update LastHeartbeat, CurrentLoad, Status)

- [ ] **JobTimeoutMonitor.cs** - Background service (Leader only)
  - Run every 10 seconds
  - Monitoring flow:
    1. Query running jobs with timeout configured
    2. If `StartedAt + Timeout < NOW` → mark job as `TimedOut`
    3. Publish `JobCancelMessage` to `job.{jobId}.cancel` channel

### Verification

- [ ] Enqueue job → verify created in DB and dispatched
- [ ] Worker sends heartbeat → verify WorkerEntity updated
- [ ] Job exceeds timeout → verify marked as timed out and cancel message sent

---

## Stage 10: Follower Coordination (Ownership)

> **Goal**: Distribute job event processing across Follower nodes
> 
> **Folder**: `Orchestrix.Coordinator/Ownership/`
> 
> **Files**: 7
> 
> **Complexity**: **HIGH** - Critical for scalability

### Problem & Solution

**Problem**: Global `job.status`/`job.logs` channels with load balancing → events fragmented across nodes → no single node has full job context

**Solution**: Job Assignment Channel → one Follower owns each job and subscribes to job-specific channels

### Implementation

- [ ] **IJobOwnershipRegistry.cs** / **JobOwnershipRegistry.cs**
  - In-memory tracking using `ConcurrentDictionary<Guid, JobOwnershipInfo>`
  - Methods: `ClaimAsync(jobId)`, `ReleaseAsync(jobId)`, `GetOwnedJobsAsync()`

- [ ] **JobLoadInfo.cs** - Load tracking model
  - Properties: `JobCount`, `LastUpdated`

- [ ] **JobAssignmentPublisher.cs**
  - Publish `JobAssignedMessage` to `job.assigned` channel
  - Called by `JobDispatcher` after dispatching job

- [ ] **JobAssignmentSubscriber.cs** - Background service
  - Subscribe to `job.assigned` with **Consumer Group** (competing consumers)
  - Processing flow when message received:
    1. Claim ownership in `JobOwnershipRegistry`
    2. Subscribe to `job.{jobId}.status` channel
    3. Subscribe to `job.{jobId}.logs` channel
    4. Update database: set `job.FollowerNodeId = this.NodeId`

- [ ] **JobEventProcessor.cs** - Event processing logic
  - Process status events: update job status and timestamps in database
  - Process log events: append to `ILogStore`

- [ ] **JobOwnershipCleanup.cs**
  - Triggered when job completes
  - Actions: release ownership, unsubscribe from job-specific channels

- [ ] **JobLoadBalancer.cs** (Optional)
  - Monitor load distribution across Follower nodes
  - Trigger rebalancing if imbalance detected (threshold: 2x difference)

### Verification

- [ ] Dispatch job → verify exactly one Follower claims ownership
- [ ] Worker publishes status → verify correct Follower processes event
- [ ] Worker publishes logs → verify logs saved to database
- [ ] Job completes → verify ownership released and channels unsubscribed

---

## Stage 11: Clustering & Scale Down

> **Goal**: Handle graceful shutdown and crash recovery
> 
> **Folder**: `Orchestrix.Coordinator/Clustering/`
> 
> **Files**: 9
> 
> **Complexity**: **VERY HIGH** - Most complex feature

### Scenarios

**Scenario A - Graceful Shutdown (SIGTERM)**:
1. Set node state to DRAINING
2. Unsubscribe from `job.assigned` (stop accepting new jobs)
3. Handoff all owned jobs to other nodes
4. Wait for acknowledgments with timeout
5. Shutdown gracefully

**Scenario B - Crash Recovery**:
1. Detect dead node (no heartbeat > 30s)
2. Leader queries orphaned jobs (jobs owned by dead node)
3. Re-publish orphaned jobs to `job.handoff` channel for reassignment

### Implementation

- [ ] **CoordinatorNodeInfo.cs** - Node information model
  - Properties: `NodeId`, `Role`, `JobCount`, `LastHeartbeat`, `Status`

- [ ] **ICoordinatorNodeRegistry.cs** / **CoordinatorNodeRegistry.cs**
  - Track active coordinator nodes
  - Detect and mark dead nodes

- [ ] **CoordinatorHeartbeatService.cs** - Background service
  - Publish heartbeat every 10 seconds to `coordinator.heartbeat` channel
  - Include: `NodeId`, `JobCount`, `Timestamp`

- [ ] **CoordinatorHealthMonitor.cs** - Background service
  - Subscribe to `coordinator.heartbeat` channel
  - Track last heartbeat timestamp per node
  - Mark node as DEAD if no heartbeat received for > 30 seconds

- [ ] **CoordinatorDrainService.cs** - Graceful shutdown handler
  - Handle SIGTERM signal
  - Drain flow:
    1. Unsubscribe from `job.assigned` channel
    2. Retrieve owned jobs from `JobOwnershipRegistry`
    3. For each job: call `JobHandoffPublisher.HandoffAsync`
    4. Wait for acknowledgments (with timeout)
    5. Proceed with shutdown

- [ ] **JobHandoffPublisher.cs** - Handoff coordination
  - Publish `JobHandoffMessage` to `job.handoff` channel
  - Wait for ACK with timeout (5s), retry up to 3 times
  - Track pending ACKs using `ConcurrentDictionary<Guid, TaskCompletionSource<bool>>`
  - Method: `HandoffAsync(jobId, reason)` → returns bool (success/failure)

- [ ] **JobHandoffSubscriber.cs** - Background service
  - Subscribe to `job.handoff` channel with Consumer Group (competing consumers)
  - Processing flow when message received:
    1. Claim ownership of handed-off job
    2. Subscribe to job-specific channels
    3. Update database: set `job.FollowerNodeId` to this node
    4. Send ACK to `job.handoff.ack.{originalNodeId}` channel

- [ ] **JobHandoffAckListener.cs** - ACK receiver
  - Subscribe to `job.handoff.ack.{nodeId}` channel (own node ID)
  - Receive acknowledgments and notify `JobHandoffPublisher`

- [ ] **OrphanJobDetector.cs** - Background service (Leader only)
  - Run every 30 seconds
  - Detection flow:
    1. Retrieve dead nodes from `CoordinatorNodeRegistry`
    2. Query jobs where `FollowerNodeId = deadNode.NodeId` AND `Status = Running`
    3. For each orphaned job: re-publish to `job.handoff` channel for reassignment

### Verification

- [ ] Graceful shutdown: Send SIGTERM → verify jobs handed off → verify ACKs received → verify clean shutdown
- [ ] Crash recovery: Kill node forcefully → verify orphans detected within 30s → verify jobs reassigned
- [ ] All nodes dead scenario: Verify hard timeout triggers → force shutdown

---

## Stage 12: Channel Cleanup

> **Goal**: Cleanup job-specific channels after job completion
> 
> **Folder**: `Orchestrix.Coordinator/ChannelCleanup/`
> 
> **Files**: 2

### Implementation

- [ ] **ChannelCleanupOptions.cs**
  - Properties: `CleanupDelay` (default: 30s), `ScanInterval` (default: 10s), `BatchSize` (default: 100)

- [ ] **ChannelCleanupScanner.cs** - Background service (Leader only)
  - Run every `ScanInterval` seconds
  - Cleanup flow:
    1. Query: Jobs where Status IN (Completed, Failed) AND ChannelsCleaned = false AND CompletedAt + CleanupDelay < NOW
    2. For each job in batch:
       - Close channel `job.{jobId}.status`
       - Close channel `job.{jobId}.logs`
       - Update database: set `job.ChannelsCleaned = true`

### Verification

- [ ] Complete job → wait CleanupDelay duration → verify channels closed
- [ ] Verify `ChannelsCleaned` flag set to true in database

---

## Stage 13: Coordinator-to-Coordinator Communication

> **Goal**: Enable direct communication between Coordinator nodes
> 
> **Folder**: `Orchestrix.Coordinator/Coordination/`
> 
> **Files**: 3

### Implementation

- [ ] **ICoordinatorCoordinator.cs**
  - Methods: `SendToNodeAsync(targetNodeId, message)`, `BroadcastAsync(message)`

- [ ] **CoordinatorCoordinator.cs** - Direct messaging implementation
  - Implement point-to-point and broadcast messaging between coordinators
  - Use channels: `coordinator.{nodeId}.messages` (point-to-point), `coordinator.broadcast` (broadcast)

- [ ] **CoordinatorCoordinatorOptions.cs**
  - Communication configuration settings

### Verification

- [ ] Send message from Coordinator 1 to Coordinator 2 → verify received
- [ ] Broadcast message → verify all nodes receive

---

## Implementation Summary

| Stage | Files | Complexity | Priority | Dependencies |
|:------|:------|:-----------|:---------|:-------------|
| 1. Persistence Abstractions | 16 | Medium | **Critical** | None |
| 2. Communication Abstractions | 4 | Low | **Critical** | None |
| 3. Core Services | 6 | Low | **Critical** | Stages 1, 2 |
| 4. Caching | 3 | Low | High | Stage 3 |
| 5. Leader Election | 3 | **High** | **Critical** | Stage 3, Locking |
| 6. Scheduling | 5 | **High** | **Critical** | Stages 3, 4, 5 |
| 7. Dispatching | 2 | Medium | **Critical** | Stages 3, 6 |
| 8. Rate Limiting | 3 | Medium | Medium | Stage 3 |
| 9. Event Handlers | 3 | Medium | **Critical** | Stages 3, 7 |
| 10. Follower Coordination | 7 | **High** | **Critical** | Stages 3, 7, 9 |
| 11. Clustering & Scale Down | 9 | **Very High** | **Critical** | Stages 3, 5, 10 |
| 12. Channel Cleanup | 2 | Low | High | Stages 3, 5 |
| 13. Coordinator Communication | 3 | Medium | Low | Stage 3 |

**Total**: ~66 files

---

## Recommended Implementation Order

**Phase 1 - Foundation** (Stages 1-3):
- Essential infrastructure and abstractions
- Low risk, straightforward implementation

**Phase 2 - Core Functionality** (Stages 4-6):
- Leader election, scheduling, dispatching
- High complexity but critical for basic operation

**Phase 3 - Event Processing** (Stages 7-9):
- Rate limiting, event handlers, follower coordination
- Enables distributed event processing

**Phase 4 - Production Readiness** (Stages 10-12):
- Clustering, cleanup, inter-coordinator communication
- Required for production deployment

**Critical Path**: Stages 1, 2, 4, 5, 6, 8, 9, 10 are mandatory for core functionality.

**Can Defer**: Stages 3, 7, 11, 12 can be implemented later or skipped for MVP.
