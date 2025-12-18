# Phase 2: Transport

> Message transport abstractions and implementations.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Transport.Abstractions` | netstandard2.1 | Orchestrix.Abstractions |
| `Orchestrix.Transport.Redis` | netstandard2.1 | Transport.Abstractions, StackExchange.Redis |
| `Orchestrix.Transport.RabbitMQ` | netstandard2.1 | Transport.Abstractions, RabbitMQ.Client |
| `Orchestrix.Transport.Kafka` | netstandard2.1 | Transport.Abstractions, Confluent.Kafka |

---

## 2.1 Transport.Abstractions

### Interfaces
- [ ] `ITransport.cs`
  ```csharp
  public interface ITransport
  {
      IPublisher Publisher { get; }
      ISubscriber Subscriber { get; }
  }
  ```

- [ ] `IPublisher.cs`
  ```csharp
  public interface IPublisher
  {
      Task PublishAsync<T>(string channel, T message, CancellationToken ct = default);
  }
  ```

- [ ] `ISubscriber.cs`
  ```csharp
  public interface ISubscriber
  {
      // Broadcast/Topic - all subscribers receive message
      Task SubscribeAsync<T>(
          string channel, 
          Func<T, Task> handler, 
          CancellationToken ct = default);
      
      // Competing Consumers - only one subscriber receives message
      Task SubscribeCompetingAsync<T>(
          string channel,
          string consumerGroup,
          string consumerName,
          Func<T, Task> handler,
          CancellationToken ct = default);
      
      // Competing Consumers with options (AutoAck, PrefetchCount, ClaimTimeout)
      Task SubscribeCompetingAsync<T>(
          string channel,
          string consumerGroup,
          string consumerName,
          Func<T, Task> handler,
          SubscriptionOptions options,
          CancellationToken ct = default);
      
      Task UnsubscribeAsync(string channel);
      
      // Close channel and cleanup resources (Redis: XTRIM/DELETE, InMemory: remove)
      Task CloseChannelAsync(string channel, CancellationToken ct = default);
  }
  ```

### Models
- [ ] `SubscriptionOptions.cs`
  ```csharp
  public record SubscriptionOptions
  {
      public bool AutoAck { get; init; } = true;
      public int PrefetchCount { get; init; } = 1;
      public TimeSpan? ClaimTimeout { get; init; }  // For pending message recovery
  }
  ```

### Serialization
- [ ] `IMessageSerializer.cs`
  ```csharp
  public interface IMessageSerializer
  {
      byte[] Serialize<T>(T message);
      T? Deserialize<T>(byte[] data);
      object? Deserialize(byte[] data, Type type);
  }
  ```

- [ ] `JsonMessageSerializer.cs` - Default implementation using System.Text.Json
  ```csharp
  public class JsonMessageSerializer : IMessageSerializer { ... }
  ```

- [ ] `MessageEnvelope.cs`
  ```csharp
  public record MessageEnvelope<T>
  {
      public required T Payload { get; init; }
      public required string MessageId { get; init; }
      public required DateTimeOffset Timestamp { get; init; }
      public string? CorrelationId { get; init; }
  }
  ```

### Messages
> Message types for communication between ControlPanel ↔ Coordinator ↔ Worker

#### Job Messages
- [ ] `JobEnqueueMessage.cs` - ControlPanel → Coordinator: request to enqueue job
- [ ] `JobAssignedMessage.cs` - Coordinator → Follower: job assigned to follower node
- [ ] `JobDispatchMessage.cs` - Coordinator → Worker: dispatch job for execution
- [ ] `JobCancelMessage.cs` - ControlPanel/Coordinator → Worker: request to cancel job
- [ ] `JobStatusMessage.cs` - Worker → Coordinator/ControlPanel: job status update
- [ ] `JobLogMessage.cs` - Worker → ControlPanel: realtime log streaming
- [ ] `JobResultMessage.cs` - Worker → Coordinator: execution result (success/fail)
- [ ] `JobHandoffMessage.cs` - Coordinator → Coordinator: handoff jobs during scale down
- [ ] `JobHandoffAckMessage.cs` - Coordinator → Coordinator: acknowledge handoff received

#### Worker Messages
- [ ] `WorkerHeartbeatMessage.cs` - Worker → Coordinator: heartbeat + capacity info
- [ ] `WorkerRegistrationMessage.cs` - Worker → Coordinator: register new worker

#### Coordinator Messages
- [ ] `CoordinatorHeartbeatMessage.cs` - Coordinator → Cluster: heartbeat between nodes

### Static Helpers
- [ ] `TransportChannels.cs`
  ```csharp
  public static class TransportChannels
  {
      public static class Job
      {
          public static string Dispatch(string queue) => $"orchestrix.job.dispatch.{queue}";
          public const string Cancel = "orchestrix.job.cancel";
          public const string Assigned = "orchestrix.job.assigned";
          public static string Status(Guid jobId) => $"orchestrix.job.{jobId}.status";
          public static string Logs(Guid jobId) => $"orchestrix.job.{jobId}.logs";
          public const string Handoff = "orchestrix.job.handoff";
          public static string HandoffAck(string nodeId) => $"orchestrix.job.handoff.ack.{nodeId}";
          public const string Enqueue = "orchestrix.job.enqueue";
      }
      
      public static class Worker
      {
          public const string Heartbeat = "orchestrix.worker.heartbeat";
          public const string Registration = "orchestrix.worker.registration";
      }
      
      public static class Coordinator
      {
          public const string Heartbeat = "orchestrix.coordinator.heartbeat";
      }
  }
  ```

**Files: 18**

---

## 2.2 Transport.Redis

- [ ] `RedisTransportOptions.cs`
  ```csharp
  public class RedisTransportOptions
  {
      public string ConnectionString { get; set; } = "localhost:6379";
      public int StreamMaxLength { get; set; } = 10000;
      public string ConsumerGroupPrefix { get; set; } = "orchestrix";
  }
  ```

- [ ] `RedisTransport.cs`
- [ ] `RedisPublisher.cs`
  - Use `XADD` for queues (Redis Streams)
  - Use `PUBLISH` for broadcast
- [ ] `RedisSubscriber.cs`
  - Use `XREADGROUP` for competing consumers
  - Use `SUBSCRIBE` for broadcast
- [ ] `ServiceCollectionExtensions.cs`

**Files: 5**

---

## 2.3 Transport.RabbitMQ

- [ ] `RabbitMQTransportOptions.cs`
  ```csharp
  public class RabbitMQTransportOptions
  {
      public string HostName { get; set; } = "localhost";
      public string UserName { get; set; } = "guest";
      public string Password { get; set; } = "guest";
      public string VirtualHost { get; set; } = "/";
      public int Port { get; set; } = 5672;
  }
  ```

- [ ] `RabbitMQTransport.cs`
- [ ] `RabbitMQPublisher.cs`
- [ ] `RabbitMQSubscriber.cs`
- [ ] `ServiceCollectionExtensions.cs`

**Files: 5**

---

## 2.4 Transport.Kafka

- [ ] `KafkaTransportOptions.cs`
  ```csharp
  public class KafkaTransportOptions
  {
      public string BootstrapServers { get; set; } = "localhost:9092";
      public string GroupId { get; set; } = "orchestrix";
  }
  ```

- [ ] `KafkaTransport.cs`
- [ ] `KafkaPublisher.cs`
- [ ] `KafkaSubscriber.cs`
- [ ] `ServiceCollectionExtensions.cs`

**Files: 5**

---

## Verification
```bash
dotnet build src/Transport/Orchestrix.Transport.Abstractions/
dotnet build src/Transport/Orchestrix.Transport.Redis/
dotnet build src/Transport/Orchestrix.Transport.RabbitMQ/
dotnet build src/Transport/Orchestrix.Transport.Kafka/
```

---

## Unit Tests

- [ ] `JsonMessageSerializerTests.cs`
  - Serialize/deserialize complex objects
  - Null handling
  - Type preservation
- [ ] `MessageEnvelopeTests.cs`
  - Envelope creation with metadata
  - Timestamp validation

---

## Summary
| Project | Files |
|:--------|:------|
| Transport.Abstractions | 18 |
| Transport.Redis | 5 |
| Transport.RabbitMQ | 5 |
| Transport.Kafka | 5 |
| Unit Tests | 2 |
| **Total** | **35** |
