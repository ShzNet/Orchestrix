# Phase 3: Locking

> **Status**: ✅ **COMPLETE**  
> Distributed locking abstractions and implementations.

## Projects
| Project | Target | Dependencies |
|:--------|:-------|:-------------|
| `Orchestrix.Locking.Abstractions` | netstandard2.1 | None |
| `Orchestrix.Locking.InMemory` | netstandard2.1 | Locking.Abstractions |
| `Orchestrix.Locking.Redis` | netstandard2.1 | Locking.Abstractions, StackExchange.Redis |

---

## Folder Structure

### Locking.Abstractions ✅
```
src/Locking/Orchestrix.Locking.Abstractions/
├── Orchestrix.Locking.Abstractions.csproj
└── Orchestrix/Locking/
    ├── IDistributedLock.cs
    ├── IDistributedLockProvider.cs
    └── DistributedLockOptions.cs
```

**Files: 4 total** (1 project + 3 interfaces/classes)

---

## 3.1 Locking.Abstractions ✅

- [x] `IDistributedLock.cs`
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

- [x] `IDistributedLockProvider.cs`
  ```csharp
  public interface IDistributedLockProvider
  {
      IDistributedLock CreateLock(string resource, DistributedLockOptions? options = null);
  }
  ```

- [x] `DistributedLockOptions.cs`
  ```csharp
  public class DistributedLockOptions
  {
      public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromSeconds(30);
  }
  ```

**Files: 4** (1 project + 3 files)
**Build:** ✅ Success (0 warnings, 0 errors)

---

## 3.2 Locking.InMemory ✅

- [x] `InMemoryLock.cs`
  - Uses `SemaphoreSlim(1,1)` per resource
  - Stored in `ConcurrentDictionary<string, SemaphoreSlim>`
  - Auto-release on dispose

- [x] `InMemoryLockProvider.cs`
  - Factory with semaphore pooling
  - Thread-safe resource management

- [x] `ServiceCollectionExtensions.cs`
  ```csharp
  public static IServiceCollection AddInMemoryLocking(this IServiceCollection services)
  ```

**Files: 4** (1 project + 3 files)
**Build:** ✅ Success (0 warnings, 0 errors)

---

## 3.3 Locking.Redis ✅

- [x] `RedisLockOptions.cs`
  ```csharp
  public class RedisLockOptions
  {
      public string ConnectionString { get; set; } = "localhost:6379";
      public string KeyPrefix { get; set; } = "orchestrix:lock:";
  }
  ```

- [x] `RedisLock.cs`
  ```csharp
  // Use SET NX EX pattern
  // SET resource:lock <token> NX EX <ttl>
  // Extend: SET resource:lock <token> XX EX <ttl>
  // Release: DEL if token matches (Lua script)
  ```

- [x] `RedisLockProvider.cs`
- [x] `ServiceCollectionExtensions.cs`

**Files: 5** (1 project + 4 files)
**Build:** ✅ Success (0 warnings, 0 errors)

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
