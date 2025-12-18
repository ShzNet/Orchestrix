# Orchestrix Coordinator Cluster Architecture

> Detailed architecture of Coordinator Cluster with Leader Election, Follower Coordination, and Scale Down mechanisms.

---

## 1. Cluster Roles & Topology

Orchestrix Coordinator operates in **Hybrid Active-Active Cluster** mode.

| Role | Who? | Responsibility |
|:-----|:-----|:---------------|
| **Leader** | 1 Node (Elected) | Scheduling, Dispatching, Timeout Monitoring |
| **Follower** | All Nodes | Event Processing (Job Status/Logs), Load Balanced |

```
┌────────────────────────────────────────────────────────────────────────┐
│                        COORDINATOR CLUSTER                             │
├────────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐            │
│  │  Coordinator 1 │  │  Coordinator 2 │  │  Coordinator 3 │            │
│  │   (LEADER)     │  │  (Follower)    │  │  (Follower)    │            │
│  │                │  │                │  │                │            │
│  │  • Schedule    │  │  • Process     │  │  • Process     │            │
│  │  • Dispatch    │  │    Events      │  │    Events      │            │
│  │  • Monitor     │  │                │  │                │            │
│  └───────┬────────┘  └───────┬────────┘  └───────┬────────┘            │
│          │                   │                   │                     │
│          └───────────────────┼───────────────────┘                     │
│                              │                                         │
│                      Distributed Lock                                  │
│                (orchestrix:coordinator:leader)                         │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

### Leader Election
- Nodes compete for distributed lock `orchestrix:coordinator:leader`
- Lease duration: 30s, Renew every 10s
- If Leader fails to renew, another node takes over

---

## 2. Communication Channels

| Channel | Type | Purpose |
|:--------|:-----|:--------|
| `orchestrix.job.dispatch.{queue}` | Queue | Dispatch jobs to workers |
| `orchestrix.job.cancel` | Queue | Cancel running jobs |
| `orchestrix.job.assigned` | Queue (Competing) | Assign job ownership |
| `orchestrix.job.{jobId}.status` | Queue | Job-specific status |
| `orchestrix.job.{jobId}.logs` | Queue | Job-specific logs |
| `orchestrix.job.handoff` | Queue (Competing) | Job handoff during scale down |
| `orchestrix.job.handoff.ack.{nodeId}` | Topic | ACK handoff |
| `orchestrix.worker.heartbeat` | Topic | Worker heartbeats |
| `orchestrix.coordinator.heartbeat` | Topic | Coordinator heartbeats |

---

## 3. Follower Coordination

### Problem
Global job.status/logs channels with load balancing → events fragmented across nodes → no single node has full job context.

### Solution: Job Assignment Channel

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                     FOLLOWER COORDINATION PATTERN                           │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. Leader dispatches job                                                   │
│     ┌────────────────┐                                                      │
│     │     LEADER     │──(Dispatch)──► job.dispatch.{queue}                  │
│     │  Coordinator 1 │                                                      │
│     │                │──(Publish)──► job.assigned                           │
│     └────────────────┘   { JobId: "abc", HistoryId: "..." }                 │
│                                                                             │
│  2. Followers compete for assignment                                        │
│     ┌────────────────┐  ┌────────────────┐  ┌────────────────┐              │
│     │  Coordinator 1 │  │  Coordinator 2 │  │  Coordinator 3 │              │
│     └───────┬────────┘  └───────┬────────┘  └───────┬────────┘              │
│             │                   │                   │                       │
│             └───────────────────┼───────────────────┘                       │
│                                 │                                           │
│                    Coordinator 2 wins (Consumer Group)                      │
│                                                                             │
│  3. Winner subscribes to job-specific channels                              │
│     ┌─────────────────────────────────────────────────────────────────────┐ │
│     │                    Coordinator 2 (Owner)                            │ │
│     │                                                                     │ │
│     │  Subscribe to:                                                      │ │
│     │    • orchestrix.job.{jobId}.status                                  │ │
│     │    • orchestrix.job.{jobId}.logs                                    │ │
│     │                                                                     │ │
│     │  Coordinator 2 now has FULL CONTEXT of this job                     │ │
│     └─────────────────────────────────────────────────────────────────────┘ │
│                                                                             │
│  4. Worker publishes to job-specific channels                               │
│     Worker ──► job.{jobId}.status ──► Coordinator 2 processes, updates DB   │
│     Worker ──► job.{jobId}.logs   ──► Coordinator 2 aggregates logs         │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Benefits
- **Single source of truth**: One node handles all events for a job
- **Full context**: Logs and status aggregated in one place
- **Load balanced**: Job processing distributed across Coordinator nodes
- **Scalable**: Adding Coordinator nodes increases throughput

### Load Balancing Strategy

| Layer | Mechanism | Description |
|:------|:----------|:------------|
| **L1: Consumer Group** | Redis Streams | Auto round-robin between consumers in group |
| **L2: Load Tracking** | Heartbeat | Each node publishes load (jobCount) in heartbeat |
| **L3: Rebalancing** | Optional | When imbalance detected, handoff jobs from overloaded node |

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    LOAD BALANCING FLOW                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. Consumer Group distributes job.assigned messages                        │
│     ┌──────────┐  ┌──────────┐  ┌──────────┐                               │
│     │  Coord 1 │  │  Coord 2 │  │  Coord 3 │                               │
│     │ (50 jobs)│  │ (48 jobs)│  │ (52 jobs)│  ← Balanced                   │
│     └──────────┘  └──────────┘  └──────────┘                               │
│                                                                             │
│  2. Each node publishes load in heartbeat                                   │
│     { NodeId: "coord-1", JobCount: 50, Timestamp: ... }                    │
│                                                                             │
│  3. If imbalance detected (threshold: 2x difference)                        │
│     ┌──────────┐  ┌──────────┐                                              │
│     │  Coord 1 │  │  Coord 2 │                                              │
│     │(100 jobs)│  │ (20 jobs)│  ← Imbalanced!                              │
│     └────┬─────┘  └──────────┘                                              │
│          │                                                                  │
│          └──► Handoff 40 jobs to job.handoff channel                       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 4. Scale Down / Job Handoff

### Scenario A: Graceful Shutdown (SIGTERM)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                       GRACEFUL SCALE DOWN FLOW                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. Coordinator 2 receives SIGTERM                                          │
│     ┌────────────────┐                                                      │
│     │  Coordinator 2 │ ◄── SIGTERM                                          │
│     │   (Draining)   │                                                      │
│     │                │ Owns: Job-A, Job-B, Job-C                            │
│     └───────┬────────┘                                                      │
│             │                                                               │
│  2. Enter DRAINING state                                                    │
│     • Unsubscribe from job.assigned (stop accepting new jobs)               │
│     • Continue processing current events                                    │
│             │                                                               │
│  3. Publish owned jobs to handoff channel                                   │
│             ├──► job.handoff { JobId: "A", Reason: "Shutdown" }             │
│             ├──► job.handoff { JobId: "B", Reason: "Shutdown" }             │
│             └──► job.handoff { JobId: "C", Reason: "Shutdown" }             │
│                                                                             │
│  4. Other nodes compete for handoff                                         │
│     ┌────────────────┐  ┌────────────────┐                                  │
│     │  Coordinator 1 │  │  Coordinator 3 │ ◄── Consumer Group               │
│     └───────┬────────┘  └───────┬────────┘                                  │
│             │                   │                                           │
│     Coord 1 wins A, C     Coord 3 wins B                                    │
│             │                   │                                           │
│  5. New owners subscribe to job channels, send ACK                          │
│             │                   │                                           │
│             └──► ACK { JobId: "A", NewOwner: "Coord1" } ──► Coord 2         │
│             └──► ACK { JobId: "C", NewOwner: "Coord1" } ──► Coord 2         │
│                                 └──► ACK { JobId: "B" } ──► Coord 2         │
│                                                                             │
│  6. Coordinator 2 waits for ACKs, then shuts down                           │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Hybrid ACK Strategy

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        HYBRID ACK DECISION TREE                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Case A: ACK received within 5s                                        │  │
│  │   → Remove job from registry                                          │  │
│  │   → Proceed to next job                                               │  │
│  │   → CLEAN HANDOFF ✓                                                   │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Case B: No ACK after 5s                                               │  │
│  │   → Retry publish (max 3 times, 15s total)                            │  │
│  │   → If ACK received → Case A                                          │  │
│  │   → If still no ACK → Case C                                          │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Case C: No ACK after all retries                                      │  │
│  │   → Mark job as ORPHAN_PENDING                                        │  │
│  │   → Continue to next job                                              │  │
│  │   → After all jobs: Wait 30s, force shutdown                          │  │
│  │   → OrphanJobDetector (Leader) will pick up later                     │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Case D: All other nodes dead                                          │  │
│  │   → Hard timeout 60s                                                  │  │
│  │   → Force shutdown                                                    │  │
│  │   → Jobs orphaned, recovered when cluster healthy                     │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Scenario B: Unexpected Crash

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         CRASH RECOVERY FLOW                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. Coordinator 2 crashes unexpectedly                                      │
│     ┌────────────────┐                                                      │
│     │  Coordinator 2 │ ◄── CRASH                                            │
│     │     (Dead)     │                                                      │
│     │                │ Owned: Job-A, Job-B, Job-C (orphaned)                │
│     └────────────────┘                                                      │
│                                                                             │
│  2. Other nodes detect missing heartbeat (30s timeout)                      │
│     Coord 1, Coord 3 monitor coordinator.heartbeat                          │
│     Coordinator 2 not seen for 30s → Declared DEAD                          │
│                                                                             │
│  3. Leader queries orphaned jobs                                            │
│     ┌────────────────┐                                                      │
│     │     LEADER     │──► Query: Jobs where OwnerNode = "Coord2"            │
│     │  Coordinator 1 │    AND Status = Running                              │
│     └────────────────┘                                                      │
│                                                                             │
│  4. Leader re-publishes orphaned jobs                                       │
│     Leader ──► job.handoff { JobId: "A", Reason: "NodeCrash" }              │
│     Leader ──► job.handoff { JobId: "B", Reason: "NodeCrash" }              │
│     Leader ──► job.handoff { JobId: "C", Reason: "NodeCrash" }              │
│                                                                             │
│  5. Remaining nodes compete for handoff (same as graceful)                  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 5. Key Components

| Component | Responsibility |
|:----------|:---------------|
| `CoordinatorHeartbeatService` | Publish heartbeat every 10s |
| `CoordinatorHealthMonitor` | Track coordinator nodes, detect dead nodes |
| `CoordinatorDrainService` | Handle SIGTERM, trigger handoff |
| `JobHandoffPublisher` | Publish handoff messages, wait ACK |
| `JobHandoffSubscriber` | Compete for handoff, take ownership |
| `JobHandoffAckListener` | Listen for ACKs |
| `OrphanJobDetector` | (Leader) Query and re-assign orphaned jobs |
| `IJobOwnershipStore` | Persist NodeId → [JobId] mapping |

---

## 6. Configuration

| Config | Default | Description |
|:-------|:--------|:------------|
| `HeartbeatInterval` | 10s | Coordinator heartbeat publish interval |
| `DeadNodeTimeout` | 30s | Time before declaring node dead |
| `HandoffAckTimeout` | 5s | Time to wait for ACK before retry |
| `HandoffMaxRetries` | 3 | Max retry attempts |
| `HandoffHardTimeout` | 60s | Max time before force shutdown |
| `OrphanCheckInterval` | 30s | How often Leader checks orphans |

---

## 7. Failure Scenarios

### Leader Crashes
- **Impact**: Scheduling stops temporarily
- **Recovery**: Lock expires (30s), another node becomes Leader
- **Data Loss**: None (schedules in DB)

### Follower Crashes
- **Impact**: Jobs owned by crashed node become orphaned
- **Recovery**: OrphanJobDetector re-assigns jobs
- **Data Loss**: None (messages in job-specific queues)

### All Coordinators Crash
- **Impact**: System downtime, workers cannot report
- **Recovery**: Coordinators restart, consume backlog from queues
- **Data Loss**: None (eventually consistent)
