# Phase 6: Persistence

> Persistence implementations for Coordinator entities.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Persistence.InMemory` | netstandard2.1 | Coordinator.Persistence.Abstractions |
| `Orchestrix.Persistence.EfCore` | net10.0 | Coordinator.Persistence.Abstractions, Microsoft.EntityFrameworkCore |

---

## Folder Structure

### Persistence.InMemory
```
src/Persistence/Orchestrix.Persistence.InMemory/
├── Orchestrix.Persistence.InMemory.csproj
└── Orchestrix/
    └── Persistence/
        └── InMemory/
            ├── InMemoryJobStore.cs
            ├── InMemoryJobHistoryStore.cs
            ├── InMemoryCronScheduleStore.cs
            ├── InMemoryIntervalScheduleStore.cs
            ├── InMemoryWorkerStore.cs
            ├── InMemoryCoordinatorNodeStore.cs
            ├── InMemoryLogStore.cs
            ├── InMemoryDeadLetterStore.cs
            └── ServiceCollectionExtensions.cs
```

### Persistence.EfCore
```
src/Persistence/Orchestrix.Persistence.EfCore/
├── Orchestrix.Persistence.EfCore.csproj
└── Orchestrix/
    └── Persistence/
        └── EfCore/
            ├── OrchestrixDbContext.cs
            ├── Configurations/
            │   ├── JobEntityConfiguration.cs
            │   ├── JobHistoryEntityConfiguration.cs
            │   ├── CronScheduleEntityConfiguration.cs
            │   ├── IntervalScheduleEntityConfiguration.cs
            │   ├── WorkerEntityConfiguration.cs
            │   ├── CoordinatorNodeEntityConfiguration.cs
            │   ├── LogEntryConfiguration.cs
            │   └── DeadLetterEntityConfiguration.cs
            ├── EfCoreJobStore.cs
            ├── EfCoreJobHistoryStore.cs
            ├── EfCoreCronScheduleStore.cs
            ├── EfCoreIntervalScheduleStore.cs
            ├── EfCoreWorkerStore.cs
            ├── EfCoreCoordinatorNodeStore.cs
            ├── EfCoreLogStore.cs
            ├── EfCoreDeadLetterStore.cs
            └── ServiceCollectionExtensions.cs
```

**Namespaces:**
- `Orchestrix.Persistence.InMemory` - In-memory stores
- `Orchestrix.Persistence.EfCore` - EF Core implementation
- `Orchestrix.Persistence.EfCore.Configurations` - Entity configurations

---

## 6.1 Persistence.InMemory

> For testing and development only.

- [ ] `InMemoryJobStore.cs`
- [ ] `InMemoryJobHistoryStore.cs`
- [ ] `InMemoryCronScheduleStore.cs`
- [ ] `InMemoryIntervalScheduleStore.cs`
- [ ] `InMemoryWorkerStore.cs`
- [ ] `InMemoryCoordinatorNodeStore.cs`
- [ ] `InMemoryLogStore.cs`
- [ ] `ServiceCollectionExtensions.cs`

**Files: 8**

---

## 6.2 Persistence.EfCore

- [ ] `OrchestrixDbContext.cs`
  ```csharp
  public class OrchestrixDbContext : DbContext
  {
      public DbSet<JobEntity> Jobs { get; set; }
      public DbSet<JobHistoryEntity> JobHistories { get; set; }
      public DbSet<CronScheduleEntity> CronSchedules { get; set; }
      public DbSet<IntervalScheduleEntity> IntervalSchedules { get; set; }
      public DbSet<WorkerEntity> Workers { get; set; }
      public DbSet<CoordinatorNodeEntity> CoordinatorNodes { get; set; }
      public DbSet<LogEntry> Logs { get; set; }
  }
  ```

- [ ] `Configurations/` - Entity configurations
  - [ ] `JobEntityConfiguration.cs`
  - [ ] `JobHistoryEntityConfiguration.cs`
  - [ ] etc.

- [ ] Store implementations
  - [ ] `EfCoreJobStore.cs`
  - [ ] `EfCoreJobHistoryStore.cs`
  - [ ] `EfCoreCronScheduleStore.cs`
  - [ ] `EfCoreIntervalScheduleStore.cs`
  - [ ] `EfCoreWorkerStore.cs`
  - [ ] `EfCoreCoordinatorNodeStore.cs`
  - [ ] `EfCoreLogStore.cs`

- [ ] `ServiceCollectionExtensions.cs`

**Files: ~15**

---

## Unit Tests

- [ ] `InMemoryJobStoreTests.cs`
  - CRUD operations
  - Query: pending, scheduled, by follower
- [ ] `InMemoryScheduleStoreTests.cs`
  - CRUD
  - Next run updates

---

## Summary
| Project | Files |
|:--------|:------|
| Persistence.InMemory | 8 |
| Persistence.EfCore | 15 |
| Unit Tests | 2 |
| **Total** | **25** |
