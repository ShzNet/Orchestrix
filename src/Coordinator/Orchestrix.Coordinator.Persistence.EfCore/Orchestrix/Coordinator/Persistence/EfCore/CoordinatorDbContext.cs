using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore;

/// <summary>
/// Entity Framework Core database context for the Coordinator.
/// </summary>
public class CoordinatorDbContext(DbContextOptions<CoordinatorDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Set of jobs managed by the coordinator.
    /// </summary>
    public DbSet<JobEntity> Jobs  => Set<JobEntity>();

    /// <summary>
    /// History of executed jobs.
    /// </summary>
    public DbSet<JobHistoryEntity> JobHistory  => Set<JobHistoryEntity>();

    /// <summary>
    /// Scheduled cron jobs.
    /// </summary>
    public DbSet<CronScheduleEntity> CronSchedules => Set<CronScheduleEntity>();

    /// <summary>
    /// Connected workers with their health status.
    /// </summary>
    public DbSet<WorkerEntity> Workers => Set<WorkerEntity>();

    /// <summary>
    /// Coordinator nodes participating in the cluster.
    /// </summary>
    public DbSet<CoordinatorNodeEntity> CoordinatorNodes  => Set<CoordinatorNodeEntity>();

    /// <summary>
    /// Failed messages or jobs that require manual intervention.
    /// </summary>
    public DbSet<DeadLetterEntity> DeadLetters => Set<DeadLetterEntity>();

    /// <summary>
    /// operational logs.
    /// </summary>
    // LogEntry might be high volume, maybe separate store or context? Adding for now.
    public DbSet<LogEntry> Logs  => Set<LogEntry>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Basic configuration
        modelBuilder.Entity<JobEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ScheduledAt); // For polling due jobs
            entity.HasIndex(e => e.FollowerNodeId); // For follower partition
            entity.HasIndex(e => e.FollowerNodeId); // For follower partition
            entity.Property<byte[]>("Timestamp").IsConcurrencyToken(); // Optimistic Concurrency
        });

        modelBuilder.Entity<JobHistoryEntity>().HasKey(e => e.Id);
        
        modelBuilder.Entity<CronScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property<byte[]>("Timestamp").IsConcurrencyToken();
        });

        modelBuilder.Entity<WorkerEntity>(entity =>
        {
            entity.HasKey(e => e.WorkerId);
            entity.HasIndex(e => e.LastHeartbeat);
            entity.Property<byte[]>("Timestamp").IsConcurrencyToken();
        });

        modelBuilder.Entity<CoordinatorNodeEntity>(entity =>
        {
            entity.HasKey(e => e.NodeId);
            entity.Property<byte[]>("Timestamp").IsConcurrencyToken();
        });
        
        modelBuilder.Entity<DeadLetterEntity>().HasKey(e => e.Id);
        
        modelBuilder.Entity<LogEntry>().HasKey(e => e.Id);
    }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        UpdateConcurrencyTokens();
        return base.SaveChanges();
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateConcurrencyTokens();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateConcurrencyTokens()
    {
        var nowBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified && entry.State != EntityState.Added) continue;
            var versionProp = entry.Metadata.FindProperty("Timestamp");
            if (versionProp is not { IsConcurrencyToken: true }) continue;
            
            entry.Property("Timestamp").CurrentValue = nowBytes;
        }
    }
}
