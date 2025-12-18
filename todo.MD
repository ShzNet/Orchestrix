# Orchestrix - Implementation TODO

> High-level overview of implementation phases.
> See `tasks/` folder for detailed implementation of each phase.

---

## Overview

| Phase | Package | Status |
|:------|:--------|:-------|
| 0 | Project Setup | ✅ Done |
| 1 | `Orchestrix.Abstractions` | ✅ Done |
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

## Project Rules

### Namespace Structure

All projects follow a consistent namespace structure:

1. **Empty RootNamespace**: All `.csproj` files must have `<RootNamespace/>` (empty)
2. **Folder-based Namespaces**: Namespaces automatically follow folder structure
3. **Root Folder**: All code files must be inside a root folder matching the project's base namespace

**Example Structure:**

```
src/Orchestrix.Abstractions/
├── Orchestrix.Abstractions.csproj  (with <RootNamespace/>)
├── Orchestrix/                      (root namespace folder)
│   ├── Enums/                       → namespace Orchestrix.Enums
│   ├── Interfaces/                  → namespace Orchestrix.Interfaces
│   ├── DTOs/                        → namespace Orchestrix.DTOs
│   └── Attributes/                  → namespace Orchestrix.Attributes
└── Polyfills/                       (exception: System.* namespaces)
```

**Benefits:**
- Easy to extend with new namespaces (just create new folders)
- Namespace matches folder structure automatically
- Consistent across all projects
- No manual namespace configuration needed

### Project Configuration

All projects inherit from `Directory.Build.props`:
- `ImplicitUsings`: enabled
- `Nullable`: enabled
- `LangVersion`: latest
- `TreatWarningsAsErrors`: true
- `GenerateDocumentationFile`: true

### Package Management

- **Central Package Management** (CPM) enabled via `Directory.Packages.props`
- All package versions defined centrally
- Projects reference packages without version numbers

---

## Phase Details

### Phase 0: Project Setup ✅
- [x] Solution `Orchestrix.slnx`
- [x] `Directory.Build.props`, `Directory.Packages.props`
- [x] Folder structure: `src/`, `docs/`, `tasks/`

### Phase 1: Abstractions ✅
> 📄 [`tasks/phase-1-abstractions.md`](./tasks/phase-1-abstractions.md)
- [x] Core enums (JobStatus, JobPriority, LogLevel)
- [x] Job types (JobInfo, IJobContext)
- [x] Schedule types (ScheduleInfo, RetryPolicy)
- [x] Polyfills for netstandard2.1 compatibility

### Phase 2: Transport 🔄
> 📄 [`tasks/phase-2-transport.md`](./tasks/phase-2-transport.md)
- [x] **2.1 Transport.Abstractions** - 7 channels, 7 messages, no polyfills, DI support
- [ ] 2.2 Transport.Redis - Redis Streams implementation
- [ ] 2.3 Transport.RabbitMQ - RabbitMQ implementation
- [ ] 2.4 Transport.Kafka - Kafka implementation
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
│   ├── Orchestrix.Abstractions/                    # Phase 1
│   │
│   ├── Transport/                                   # Phase 2
│   │   ├── Orchestrix.Transport.Abstractions/
│   │   ├── Orchestrix.Transport.Redis/
│   │   ├── Orchestrix.Transport.RabbitMQ/
│   │   └── Orchestrix.Transport.Kafka/
│   │
│   ├── Locking/                                     # Phase 3
│   │   ├── Orchestrix.Locking.Abstractions/
│   │   ├── Orchestrix.Locking.InMemory/
│   │   └── Orchestrix.Locking.Redis/
│   │
│   ├── Coordinator/                                 # Phase 4
│   │   ├── Orchestrix.Coordinator.Abstractions/
│   │   ├── Orchestrix.Coordinator.Persistence.Abstractions/
│   │   └── Orchestrix.Coordinator/
│   │
│   ├── Worker/                                      # Phase 5
│   │   └── Orchestrix.Worker/
│   │
│   ├── Persistence/                                 # Phase 6
│   │   ├── Orchestrix.Persistence.InMemory/
│   │   └── Orchestrix.Persistence.EfCore/
│   │
│   ├── ControlPanel/                                # Phase 7
│   │   └── Orchestrix.ControlPanel/
│   │
│   ├── Api/
│   │   ├── Orchestrix.ServiceApi/                   # Phase 8
│   │   └── Orchestrix.Admin/                        # Phase 10
│   │
│   ├── Client/                                      # Phase 9
│   │   └── Orchestrix.Client/
│   │
│   └── Server/                                      # Phase 13
│       ├── Orchestrix.Server/
│       └── Orchestrix.Dashboard/
│
├── samples/                                         # Phase 12
│   ├── Orchestrix.Sample.Jobs/
│   ├── Orchestrix.Sample.Coordinator/
│   ├── Orchestrix.Sample.Worker/
│   └── Orchestrix.Sample.Api/
│
└── tests/                                           # Phase 11
    ├── Orchestrix.Tests.Unit/
    └── Orchestrix.Tests.Integration/
```
```

> **Note**: This shows the high-level project structure only.
> 
> **For detailed file structures**, see individual phase documentation:
> - Phase 1: [`tasks/phase-1-abstractions.md`](./tasks/phase-1-abstractions.md)
> - Phase 2: [`tasks/phase-2-transport.md`](./tasks/phase-2-transport.md)
> - Phase 3: [`tasks/phase-3-locking.md`](./tasks/phase-3-locking.md)
> - And so on...
>
> **Namespace Convention**: All projects follow folder-based namespace structure:
> - Each `.csproj` has `<RootNamespace/>` (empty)
> - Code organized in root folder matching project's base namespace
> - Example: `Orchestrix.Transport.Redis` → `Orchestrix/Transport/Redis/` → `namespace Orchestrix.Transport.Redis;`

