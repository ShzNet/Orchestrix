# Phase 3: Locking

> Distributed locking abstractions and implementations.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Locking.Abstractions` | netstandard2.1 | None |
| `Orchestrix.Locking.InMemory` | netstandard2.1 | Locking.Abstractions |
| `Orchestrix.Locking.Redis` | netstandard2.1 | Locking.Abstractions, StackExchange.Redis |

---

## 3.1 Locking.Abstractions

- [ ] `IDistributedLock.cs`
  ```csharp
  public interface IDistributedLock : IAsyncDisposable
  {
      string Resource { get; }
      bool IsHeld { get; }
      Task<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken ct = default);
      Task<bool> ExtendAsync(TimeSpan duration, CancellationToken ct = default);
      Task ReleaseAsync(CancellationToken ct = default);
  }
  ```

- [ ] `IDistributedLockProvider.cs`
  ```csharp
  public interface IDistributedLockProvider
  {
      IDistributedLock CreateLock(string resource, DistributedLockOptions? options = null);
  }
  ```

- [ ] `DistributedLockOptions.cs`
  ```csharp
  public record DistributedLockOptions
  {
      public TimeSpan DefaultTtl { get; init; } = TimeSpan.FromSeconds(30);
  }
  ```

**Files: 3**

---

## 3.2 Locking.InMemory

- [ ] `InMemoryLock.cs`
  ```csharp
  // Use SemaphoreSlim(1,1) per resource
  // Store in ConcurrentDictionary<string, SemaphoreSlim>
  ```

- [ ] `InMemoryLockProvider.cs`
- [ ] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddInMemoryLocking(this IServiceCollection services)
  ```

**Files: 3**

---

## 3.3 Locking.Redis

- [ ] `RedisLockOptions.cs`
  ```csharp
  public class RedisLockOptions
  {
      public string ConnectionString { get; set; } = "localhost:6379";
      public string KeyPrefix { get; set; } = "orchestrix:lock:";
  }
  ```

- [ ] `RedisLock.cs`
  ```csharp
  // Use SET NX EX pattern
  // SET resource:lock <token> NX EX <ttl>
  // Extend: SET resource:lock <token> XX EX <ttl>
  // Release: DEL if token matches (Lua script)
  ```

- [ ] `RedisLockProvider.cs`
- [ ] `ServiceCollectionExtensions.cs`

**Files: 4**

---

## Verification
```bash
dotnet build src/Locking/Orchestrix.Locking.Abstractions/
dotnet build src/Locking/Orchestrix.Locking.InMemory/
dotnet build src/Locking/Orchestrix.Locking.Redis/
```

---

## Unit Tests

- [ ] `InMemoryLockTests.cs`
  - Acquire, release, extend
  - Timeout handling
  - Concurrent access
- [ ] `InMemoryLockProviderTests.cs`
  - Create multiple locks
  - Resource isolation

---

## Summary
| Project | Files |
|:--------|:------|
| Locking.Abstractions | 3 |
| Locking.InMemory | 3 |
| Locking.Redis | 4 |
| Unit Tests | 2 |
| **Total** | **12** |
