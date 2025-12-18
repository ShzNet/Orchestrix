# Phase 11: Testing

> Integration and End-to-End tests. (Unit tests are inline in each phase.)

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Tests.Integration` | net10.0 | xUnit, Testcontainers, FluentAssertions |

---

## 11.1 Integration Tests

### End-to-End Flow
- [ ] `JobLifecycleTests.cs`
  - Enqueue → Dispatch → Execute → Complete
  - Job with delay
  - Job with timeout
  - Job cancellation

### Cluster Tests
- [ ] `LeaderFailoverTests.cs`
  - Leader election
  - Failover when leader dies
  - Split brain prevention
- [ ] `FollowerCoordinationTests.cs`
  - Job ownership sync
  - Multiple followers
- [ ] `ScaleDownHandoffTests.cs`
  - Graceful handoff
  - Force stop with reassign

### Schedule Tests
- [ ] `CronScheduleTests.cs`
  - Cron triggers job on time
  - Cron catch-up after downtime
- [ ] `IntervalScheduleTests.cs`
  - Interval triggers repeatedly

---

## 11.2 Transport Integration Tests

### Redis
- [ ] `RedisTransportTests.cs`
  - Publish/subscribe with real Redis
  - Competing consumers
  - Reconnection after disconnect

### RabbitMQ
- [ ] `RabbitMQTransportTests.cs`
  - Publish/subscribe with real RabbitMQ
  - Dead letter queue

### Kafka
- [ ] `KafkaTransportTests.cs`
  - Publish/subscribe with real Kafka
  - Consumer groups

---

## 11.3 Persistence Integration Tests

### EF Core
- [ ] `EfCoreJobStoreTests.cs`
  - CRUD with real database
  - Query performance
- [ ] `EfCoreScheduleStoreTests.cs`
  - Schedule queries

---

## 11.4 Locking Integration Tests

### Redis
- [ ] `RedisLockTests.cs`
  - Lock with real Redis
  - Lock renewal
  - Lock failover

---

## 11.5 Test Utilities

- [ ] `TestFixtures/`
  - `OrchestrixTestHost.cs` - Complete test host setup
  - `InMemoryTestContext.cs` - Fast in-memory testing
  - `TestJobHandler.cs` - Mock job handlers
  - `DockerFixtures.cs` - Testcontainers setup

---

## Summary
| Category | Count |
|:---------|:------|
| E2E Flow | 4 |
| Cluster | 6 |
| Schedule | 4 |
| Transport | 6 |
| Persistence | 4 |
| Locking | 3 |
| Utilities | 4 |
| **Total** | **~31** |
