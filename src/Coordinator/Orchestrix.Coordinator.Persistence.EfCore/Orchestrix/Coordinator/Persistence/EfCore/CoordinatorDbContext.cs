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
            entity.Property<uint>("Version").IsConcurrencyToken(); // Optimistic Concurrency
        });

        modelBuilder.Entity<JobHistoryEntity>().HasKey(e => e.Id);
        
        modelBuilder.Entity<CronScheduleEntity>().HasKey(e => e.Id);

        modelBuilder.Entity<WorkerEntity>(entity =>
        {
            entity.HasKey(e => e.WorkerId);
            entity.HasIndex(e => e.LastHeartbeat);
        });

        modelBuilder.Entity<CoordinatorNodeEntity>().HasKey(e => e.NodeId);
        
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
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                var versionProp = entry.Metadata.FindProperty("Version");
                if (versionProp != null && versionProp.IsConcurrencyToken)
                {
                    var prop = entry.Property("Version");
                    uint current = prop.CurrentValue is uint val ? val : 0u;
                    prop.CurrentValue = current + 1;
                }
            }
        }
    }
}
