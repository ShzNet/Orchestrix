# Phase 2: Transport

> **Status**: 🔄 **IN PROGRESS** (Phase 2.1 ✅ Complete)  
> Message transport abstractions and implementations.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Transport.Abstractions` | netstandard2.1 | Orchestrix.Abstractions |
| `Orchestrix.Transport.Redis` | netstandard2.1 | Transport.Abstractions, StackExchange.Redis |
| `Orchestrix.Transport.RabbitMQ` | netstandard2.1 | Transport.Abstractions, RabbitMQ.Client |
| `Orchestrix.Transport.Kafka` | netstandard2.1 | Transport.Abstractions, Confluent.Kafka |

---

## Folder Structure

### Transport.Abstractions ✅
```
src/Transport/Orchestrix.Transport.Abstractions/
├── Orchestrix.Transport.Abstractions.csproj
├── Orchestrix/Transport/
│   ├── ITransport.cs
│   ├── IPublisher.cs
│   ├── ISubscriber.cs
│   ├── TransportOptions.cs
│   ├── TransportChannels.cs
│   ├── Serialization/
│   │   ├── IMessageSerializer.cs
│   │   ├── JsonMessageSerializer.cs
│   │   └── MessageEnvelope.cs
│   └── Messages/
│       ├── Jobs/
│       │   ├── JobDispatchMessage.cs
│       │   ├── JobCancelMessage.cs
│       │   ├── JobStatusMessage.cs
│       │   └── JobLogMessage.cs
│       └── Workers/
│           ├── WorkerJoinMessage.cs
│           ├── WorkerShutdownMessage.cs
│           └── WorkerMetricsMessage.cs
```

**Files: 15 total**
- Core: 5 (ITransport, IPublisher, ISubscriber, TransportOptions, TransportChannels)
- Serialization: 3 (IMessageSerializer, JsonMessageSerializer, MessageEnvelope)
- Messages: 7 (4 Jobs + 3 Workers)

**Channels: 7 total**
- Coordinator → Worker: `JobDispatch(queue)`, `JobCancel`
- Worker → Coordinator: `JobStatus(executionId)`, `JobLog(executionId)`, `WorkerJoin`, `WorkerShutdown(workerId)`, `WorkerMetrics(workerId)`

**Key Features:**
- ✅ No polyfills (all classes)
- ✅ IServiceCollection support for DI
- ✅ Dynamic routing: ID-first pattern (`{prefix}:job:{id}:action`)
- ✅ Auto-unsubscribe: Handler returns `bool`
- ✅ Nested configuration: `services.AddCoordinator(opt => opt.Transport.UseRedis(...))`

---

## 2.1 Transport.Abstractions ✅

### Interfaces
- [x] `ITransport.cs`
  ```csharp
  public interface ITransport
  {
      IPublisher Publisher { get; }
      ISubscriber Subscriber { get; }
  }
  ```

- [x] `IPublisher.cs`
  ```csharp
  public interface IPublisher
  {
      Task PublishAsync<T>(string channel, T message, CancellationToken ct = default);
  }
  ```

- [x] `ISubscriber.cs` - Simplified with auto-unsubscribe
  ```csharp
  public interface ISubscriber
  {
      // Handler returns true to continue, false to auto-unsubscribe
      Task SubscribeAsync<T>(
          string channel, 
          Func<T, Task<bool>> handler, 
          CancellationToken ct = default);
      
      Task UnsubscribeAsync(string channel);
      Task CloseChannelAsync(string channel, CancellationToken ct = default);
  }
  ```

### Models
- [x] ~~`SubscriptionOptions.cs`~~ - Removed (not needed in abstractions)

### Serialization
- [x] `IMessageSerializer.cs`
  ```csharp
  public interface IMessageSerializer
  {
      byte[] Serialize<T>(T message);
      T? Deserialize<T>(byte[] data);
      object? Deserialize(byte[] data, Type type);
  }
  ```

- [x] `JsonMessageSerializer.cs` - Default implementation using System.Text.Json 10.0.1
  ```csharp
  public class JsonMessageSerializer : IMessageSerializer { ... }
  ```

- [x] `MessageEnvelope.cs`
  ```csharp
  public record MessageEnvelope
  {
      public required Guid MessageId { get; init; }
      public required DateTimeOffset Timestamp { get; init; }
      public required string MessageType { get; init; }
      public required byte[] Payload { get; init; }
  }
  ```

### Messages
> Message types for Worker ↔ Coordinator communication

#### Job Messages (in `Messages/Jobs/`)
- [x] ~~`JobEnqueueMessage.cs`~~ - Removed (Service API only)
- [x] ~~`JobAssignedMessage.cs`~~ - Removed (Coordinator internal)
- [x] `JobDispatchMessage.cs` - Coordinator → Worker: dispatch job for execution
- [x] `JobCancelMessage.cs` - Coordinator → Worker: request to cancel job
- [x] `JobStatusMessage.cs` - Worker → Coordinator: job status update
- [x] `JobLogMessage.cs` - Worker → Coordinator: realtime log streaming
- [x] `JobResultMessage.cs` - Worker → Coordinator: execution result (success/fail)
- [x] ~~`JobHandoffMessage.cs`~~ - Removed (Coordinator internal)
- [x] ~~`JobHandoffAckMessage.cs`~~ - Removed (Coordinator internal)

#### Worker Messages (in `Messages/Workers/`)
- [x] `WorkerHeartbeatMessage.cs` - Worker → Coordinator: heartbeat + capacity info
- [x] `WorkerRegistrationMessage.cs` - Worker → Coordinator: register new worker

#### Coordinator Messages
- [x] ~~`CoordinatorHeartbeatMessage.cs`~~ - Removed (Coordinator internal)

### Channels
- [x] `TransportChannels.cs` - Dynamic channel routing with configurable prefix
  ```csharp
  public class TransportChannels
  {
      public TransportChannels(string prefix = "orchestrix");
      
      // Queue-based (Coordinator → Worker)
      public string JobDispatch(string queueName);
      public string JobCancel { get; }
      
      // Execution-based (Worker → Coordinator)
      public string JobStatus(Guid executionId);
      public string JobLog(Guid executionId);
      public string JobResult(Guid executionId);
      
      // Static channels
      public string WorkerHeartbeat { get; }
      public string WorkerRegistration { get; }
      public string WorkerMetrics { get; }
      
      public static TransportChannels Default { get; }
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
