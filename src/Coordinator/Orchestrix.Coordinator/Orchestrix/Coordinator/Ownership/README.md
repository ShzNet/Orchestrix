# Job Assignment Publisher

## Purpose

Publishes job assignment events to followers for distributed event processing.

## Architecture

```
Leader
  ├─> job.dispatch.{queue} → Workers (execute jobs)
  └─> job.dispatched → Followers (track events)
```

## Usage

```csharp
await assignmentPublisher.PublishJobAssignedAsync(
    jobId, 
    executionId, 
    queue);
```

## Flow

1. Leader dispatches job
2. Publish to `job.dispatch.{queue}` for workers
3. Publish to `job.dispatched` for followers
4. Followers race to claim ownership
5. Winner subscribes to job-specific channels

## Benefits

✅ Separate queues for workers vs followers  
✅ Consumer groups for load balancing  
✅ Automatic cache invalidation  
✅ Clear separation of concerns
