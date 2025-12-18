# Orchestrix - Implementation TODO

> High-level overview of implementation phases.
> See `tasks/` folder for detailed implementation of each phase.

---

## Overview

| Phase | Package | Status |
|:------|:--------|:-------|
| 0 | Project Setup | ✅ Done |
| 1 | `Orchestrix.Abstractions` | 🔲 TODO |
| 2 | `Orchestrix.Transport.*` | 🔲 TODO |
| 3 | `Orchestrix.Locking.*` | 🔲 TODO |
| 4 | `Orchestrix.Coordinator.*` | 🔲 TODO |
| 5 | `Orchestrix.Worker` | 🔲 TODO |
| 6 | `Orchestrix.Persistence.*` | 🔲 TODO |
| 7 | `Orchestrix.ControlPanel` | 🔲 TODO |
| 8 | `Orchestrix.ServiceApi` | 🔲 TODO |
| 9 | `Orchestrix.Client` | 🔲 TODO |
| 10 | `Orchestrix.Admin` | 🔲 TODO |
| 11 | Testing | 🔲 TODO |
| 12 | Sample | 🔲 TODO |
| 13 | `Orchestrix.Server` | 🔲 TODO |

---

## Phase Details

### Phase 0: Project Setup ✅
- [x] Solution `Orchestrix.slnx`
- [x] `Directory.Build.props`, `Directory.Packages.props`
- [x] Folder structure: `src/`, `docs/`, `tasks/`

### Phase 1: Abstractions
> 📄 [`tasks/phase-1-abstractions.md`](./tasks/phase-1-abstractions.md)
- [ ] Core enums (JobStatus, JobPriority, LogLevel)
- [ ] Core interfaces (IJobContext, IJobHandler)
- [ ] DTOs (JobInfo, ScheduleInfo, RetryPolicy)
- [ ] Attributes (JobHandlerAttribute)

### Phase 2: Transport
> 📄 [`tasks/phase-2-transport.md`](./tasks/phase-2-transport.md)
- [ ] Transport abstractions (IPublisher, ISubscriber)
- [ ] Serialization (IMessageSerializer)
- [ ] Redis implementation
- [ ] RabbitMQ implementation
- [ ] Kafka implementation
- [ ] **Unit Tests**: `JsonMessageSerializerTests`

### Phase 3: Locking
> 📄 [`tasks/phase-3-locking.md`](./tasks/phase-3-locking.md)
- [ ] Locking abstractions
- [ ] InMemory implementation
- [ ] Redis implementation
- [ ] **Unit Tests**: `InMemoryLockTests`, `InMemoryLockProviderTests`

### Phase 4: Coordinator Core
> 📄 [`tasks/phase-4-coordinator.md`](./tasks/phase-4-coordinator.md)
> 📄 [`docs/coordinator-implementation-todo.MD`](./docs/coordinator-implementation-todo.MD)
- [ ] **Persistence.Abstractions** (entities + store interfaces)
- [ ] Core services (CoordinatorService)
- [ ] Caching layer
- [ ] Leader Election
- [ ] Scheduling & Dispatching
- [ ] Follower coordination (Ownership)
- [ ] Scale down / handoff (Clustering)
- [ ] Handlers (Enqueue, Heartbeat, Timeout)
- [ ] **Unit Tests**: `LeaderElectionTests`, `JobDispatcherTests`, `JobStateMachineTests`, `SchedulerTests`


### Phase 5: Worker Core
> 📄 [`tasks/phase-5-worker.md`](./tasks/phase-5-worker.md)
- [ ] Worker service
- [ ] Job execution
- [ ] Handler registry
- [ ] Cancellation handling
- [ ] **Unit Tests**: `JobExecutorTests`, `HandlerRegistryTests`

### Phase 6: Persistence
> 📄 [`tasks/phase-6-persistence.md`](./tasks/phase-6-persistence.md)
- [ ] InMemory stores
- [ ] EF Core stores
- [ ] Dead Letter Queue (DLQ) store
- [ ] **Unit Tests**: `InMemoryJobStoreTests`, `InMemoryScheduleStoreTests`


### Phase 7: ControlPanel
> 📄 [`tasks/phase-7-controlpanel.md`](./tasks/phase-7-controlpanel.md)
> **Core business logic library**
- [ ] `IControlPanelService` - Main service interface
- [ ] Enqueue jobs → write to storage
- [ ] Cancel jobs → communicate with Coordinator
- [ ] Query job status, schedules, workers
- [ ] Monitor running jobs (via Coordinator)
- [ ] **Unit Tests**: `ControlPanelServiceTests`

### Phase 8: Service API
> 📄 [`tasks/phase-8-serviceapi.md`](./tasks/phase-8-serviceapi.md)
> **REST API for 3rd party integration**
- [ ] Uses `IControlPanelService`
- [ ] REST controllers: Jobs, Schedules
- [ ] For external services to enqueue/cancel/query jobs
- [ ] **Unit Tests**: `JobsControllerTests`

### Phase 9: Client SDK
> 📄 [`tasks/phase-9-client.md`](./tasks/phase-9-client.md)
> **Client library connecting to Service API**
- [ ] `IOrchestrixClient` interface
- [ ] HTTP client to call Phase 8 API
- [ ] EnqueueAsync, CancelAsync, GetStatusAsync
- [ ] **Unit Tests**: `OrchestrixClientTests`

### Phase 10: Admin
> 📄 [`tasks/phase-10-admin.md`](./tasks/phase-10-admin.md)
> **Admin API + Real-time monitoring dashboard**
- [ ] Uses `IControlPanelService`
- [ ] Admin REST API (full access)
- [ ] SignalR for real-time job/worker status
- [ ] Dashboard integration support
- [ ] **Unit Tests**: `AdminControllerTests`

### Phase 11: Testing
> 📄 [`tasks/phase-11-testing.md`](./tasks/phase-11-testing.md)
- [ ] Integration tests
- [ ] End-to-end tests

### Phase 12: Sample
> 📄 [`tasks/phase-12-sample.md`](./tasks/phase-12-sample.md)
- [ ] Sample Coordinator host
- [ ] Sample Worker host
- [ ] Sample API host
- [ ] Sample job handlers

### Phase 13: Server
> 📄 [`tasks/phase-13-server.md`](./tasks/phase-13-server.md)
> **Pre-packaged all-in-one server application**
- [ ] `Orchestrix.Server` - Coordinator + Service API + Admin API + Dashboard
- [ ] Embedded SPA Dashboard (Blazor/React)
- [ ] Single executable deployment
- [ ] Docker image

---

## Implementation Order

```
Phase 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10 → 11 → 12
```

---

## Project Structure

```
Orchestrix/
├── Orchestrix.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── nuget.config
├── idea.md
├── todo.MD
│
├── docs/
│   ├── architecture-coordinator-cluster.MD
│   └── coordinator-implementation-todo.MD
│
├── tasks/
│   ├── phase-1-abstractions.md
│   ├── phase-2-transport.md
│   ├── phase-3-locking.md
│   ├── phase-4-coordinator.md
│   ├── phase-5-worker.md
│   ├── phase-6-persistence.md
│   ├── phase-7-controlpanel.md
│   ├── phase-8-serviceapi.md
│   ├── phase-9-client.md
│   ├── phase-10-admin.md
│   ├── phase-11-testing.md
│   └── phase-12-sample.md
│
├── src/
│   │
│   ├── Orchestrix.Abstractions/
│   │   ├── JobStatus.cs
│   │   ├── JobPriority.cs
│   │   ├── LogLevel.cs
│   │   ├── IJobContext.cs
│   │   ├── IJobHandler.cs
│   │   ├── JobInfo.cs
│   │   ├── ScheduleInfo.cs
│   │   └── JobHandlerAttribute.cs
│   │
│   ├── Transport/
│   │   ├── Orchestrix.Transport.Abstractions/
│   │   │   ├── ITransport.cs
│   │   │   ├── IPublisher.cs
│   │   │   ├── ISubscriber.cs
│   │   │   ├── SubscriptionOptions.cs
│   │   │   ├── IMessageSerializer.cs
│   │   │   ├── JsonMessageSerializer.cs
│   │   │   ├── MessageEnvelope.cs
│   │   │   ├── TransportChannels.cs
│   │   │   └── Messages/
│   │   │       ├── JobEnqueueMessage.cs
│   │   │       ├── JobAssignedMessage.cs
│   │   │       ├── JobDispatchMessage.cs
│   │   │       ├── JobCancelMessage.cs
│   │   │       ├── JobStatusMessage.cs
│   │   │       ├── JobLogMessage.cs
│   │   │       ├── JobResultMessage.cs
│   │   │       ├── JobHandoffMessage.cs
│   │   │       ├── JobHandoffAckMessage.cs
│   │   │       ├── WorkerHeartbeatMessage.cs
│   │   │       ├── WorkerRegistrationMessage.cs
│   │   │       └── CoordinatorHeartbeatMessage.cs
│   │   │
│   │   ├── Orchestrix.Transport.Redis/
│   │   │   ├── RedisTransport.cs
│   │   │   ├── RedisPublisher.cs
│   │   │   ├── RedisSubscriber.cs
│   │   │   ├── RedisTransportOptions.cs
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Locking/
│   │   ├── Orchestrix.Locking.Abstractions/
│   │   │   ├── IDistributedLock.cs
│   │   │   ├── IDistributedLockProvider.cs
│   │   │   └── DistributedLockOptions.cs
│   │   │
│   │   ├── Orchestrix.Locking.InMemory/
│   │   │   ├── InMemoryLock.cs
│   │   │   ├── InMemoryLockProvider.cs
│   │   │   └── ServiceCollectionExtensions.cs
│   │   │
│   │   └── Orchestrix.Locking.Redis/
│   │       ├── RedisLock.cs
│   │       ├── RedisLockProvider.cs
│   │       ├── RedisLockOptions.cs
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Coordinator/
│   │   ├── Orchestrix.Coordinator.Abstractions/
│   │   │   ├── CoordinatorOptions.cs
│   │   │   └── ICoordinatorBuilder.cs
│   │   │
│   │   ├── Orchestrix.Coordinator.Persistence.Abstractions/
│   │   │   ├── Entities/
│   │   │   │   ├── JobEntity.cs
│   │   │   │   ├── JobHistoryEntity.cs
│   │   │   │   ├── CronScheduleEntity.cs
│   │   │   │   ├── IntervalScheduleEntity.cs
│   │   │   │   ├── WorkerEntity.cs
│   │   │   │   ├── CoordinatorNodeEntity.cs
│   │   │   │   └── LogEntry.cs
│   │   │   ├── IJobStore.cs
│   │   │   ├── IJobHistoryStore.cs
│   │   │   ├── ICronScheduleStore.cs
│   │   │   ├── IIntervalScheduleStore.cs
│   │   │   ├── IWorkerStore.cs
│   │   │   ├── ICoordinatorNodeStore.cs
│   │   │   └── ILogStore.cs
│   │   │
│   │   └── Orchestrix.Coordinator/
│   │       ├── LeaderElection/
│   │       │   ├── ILeaderElection.cs
│   │       │   ├── LeaderElection.cs
│   │       │   └── LeaderElectionOptions.cs
│   │       ├── Coordination/
│   │       │   ├── ICoordinatorCoordinator.cs
│   │       │   ├── CoordinatorCoordinator.cs
│   │       │   └── CoordinatorCoordinatorOptions.cs
│   │       ├── Scheduling/
│   │       │   ├── IScheduler.cs
│   │       │   ├── CronExpressionParser.cs
│   │       │   ├── ScheduleEvaluator.cs
│   │       │   ├── ScheduleScanner.cs
│   │       │   └── JobPlanner.cs
│   │       ├── Dispatching/
│   │       │   ├── IJobDispatcher.cs
│   │       │   └── JobDispatcher.cs
│   │       ├── RateLimiting/
│   │       │   ├── IRateLimiter.cs
│   │       │   ├── RateLimitOptions.cs
│   │       │   └── SlidingWindowRateLimiter.cs
│   │       ├── Handlers/
│   │       │   ├── JobEnqueueHandler.cs
│   │       │   ├── WorkerHeartbeatHandler.cs
│   │       │   └── JobTimeoutMonitor.cs
│   │       ├── Ownership/
│   │       │   ├── IJobOwnershipRegistry.cs
│   │       │   ├── JobOwnershipRegistry.cs
│   │       │   ├── JobAssignedMessage.cs
│   │       │   ├── JobAssignmentPublisher.cs
│   │       │   ├── JobAssignmentSubscriber.cs
│   │       │   ├── JobEventProcessor.cs
│   │       │   └── JobOwnershipCleanup.cs
│   │       ├── Clustering/
│   │       │   ├── CoordinatorNodeInfo.cs
│   │       │   ├── ICoordinatorNodeRegistry.cs
│   │       │   ├── CoordinatorNodeRegistry.cs
│   │       │   ├── CoordinatorHeartbeatService.cs
│   │       │   ├── CoordinatorHealthMonitor.cs
│   │       │   ├── CoordinatorDrainService.cs
│   │       │   ├── JobHandoffMessage.cs
│   │       │   ├── JobHandoffAckMessage.cs
│   │       │   ├── JobHandoffPublisher.cs
│   │       │   ├── JobHandoffSubscriber.cs
│   │       │   ├── JobHandoffAckListener.cs
│   │       │   ├── IJobOwnershipStore.cs
│   │       │   ├── RedisJobOwnershipStore.cs
│   │       │   └── OrphanJobDetector.cs
│   │       ├── CoordinatorService.cs
│   │       ├── ICoordinatorService.cs
│   │       ├── CoordinatorOptions.cs
│   │       ├── CoordinatorClusterOptions.cs
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Worker/
│   │   └── Orchestrix.Worker/
│   │       ├── WorkerService.cs
│   │       ├── IWorkerService.cs
│   │       ├── WorkerOptions.cs
│   │       ├── WorkerInfo.cs
│   │       ├── WorkerState.cs
│   │       ├── Execution/
│   │       │   ├── IJobExecutor.cs
│   │       │   ├── JobExecutor.cs
│   │       │   └── JobContext.cs
│   │       ├── Registry/
│   │       │   ├── IJobHandlerRegistry.cs
│   │       │   └── JobHandlerRegistry.cs
│   │       ├── Consumer/
│   │       │   └── JobConsumer.cs
│   │       ├── Heartbeat/
│   │       │   └── HeartbeatService.cs
│   │       ├── Lifecycle/
│   │       │   ├── GracefulShutdownOptions.cs
│   │       │   └── WorkerDrainService.cs
│   │       ├── Cancellation/
│   │       │   └── CancellationHandler.cs
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Persistence/
│   │   ├── Orchestrix.Persistence.InMemory/
│   │   │   ├── InMemoryJobStore.cs
│   │   │   ├── InMemoryJobHistoryStore.cs
│   │   │   ├── InMemoryCronScheduleStore.cs
│   │   │   ├── InMemoryIntervalScheduleStore.cs
│   │   │   ├── InMemoryWorkerStore.cs
│   │   │   ├── InMemoryCoordinatorNodeStore.cs
│   │   │   ├── InMemoryLogStore.cs
│   │   │   └── ServiceCollectionExtensions.cs
│   │   │
│   │   └── Orchestrix.Persistence.EfCore/
│   │       ├── OrchestrixDbContext.cs
│   │       ├── Configurations/
│   │       │   └── ...EntityConfiguration.cs
│   │       ├── EfCoreJobStore.cs
│   │       ├── EfCoreJobHistoryStore.cs
│   │       ├── ... (other stores)
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── ControlPanel/
│   │   └── Orchestrix.ControlPanel/           # Phase 7
│   │       ├── IControlPanelService.cs
│   │       ├── ControlPanelService.cs
│   │       ├── ControlPanelOptions.cs
│   │       ├── Models/
│   │       │   ├── EnqueueOptions.cs
│   │       │   ├── JobQueryOptions.cs
│   │       │   ├── ScheduleQueryOptions.cs
│   │       │   ├── PaginatedResult.cs
│   │       │   ├── ClusterStatus.cs
│   │       │   └── WorkerInfo.cs
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Api/
│   │   ├── Orchestrix.ServiceApi/             # Phase 8
│   │   │   ├── Controllers/
│   │   │   │   ├── JobsController.cs
│   │   │   │   └── SchedulesController.cs
│   │   │   ├── DTOs/
│   │   │   │   ├── EnqueueRequest.cs
│   │   │   │   └── EnqueueResult.cs
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── EndpointRouteBuilderExtensions.cs
│   │   │
│   │   └── Orchestrix.Admin/                  # Phase 10
│   │       ├── Controllers/
│   │       │   ├── AdminJobsController.cs
│   │       │   ├── AdminSchedulesController.cs
│   │       │   ├── AdminWorkersController.cs
│   │       │   └── AdminClusterController.cs
│   │       ├── Hubs/
│   │       │   ├── AdminHub.cs
│   │       │   ├── IAdminNotifier.cs
│   │       │   └── SignalRAdminNotifier.cs
│   │       ├── ServiceCollectionExtensions.cs
│   │       └── EndpointRouteBuilderExtensions.cs
│   │
│   └── Client/
│       └── Orchestrix.Client/                 # Phase 9
│           ├── IOrchestrixClient.cs
│           ├── OrchestrixClient.cs
│           ├── OrchestrixClientOptions.cs
│           └── ServiceCollectionExtensions.cs
│
├── samples/
│   ├── Orchestrix.Sample.Jobs/
│   ├── Orchestrix.Sample.Coordinator/
│   ├── Orchestrix.Sample.Worker/
│   └── Orchestrix.Sample.Api/
│
└── tests/
    ├── Orchestrix.Tests.Unit/
    │   └── ...
    └── Orchestrix.Tests.Integration/
        └── ...
```

