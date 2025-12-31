using Microsoft.EntityFrameworkCore;
using Orchestrix.Persistence.EfCore;

/// <summary>
/// Sample implementation of the abstract CoordinatorDbContext.
/// You can add your own application-specific entities here.
/// </summary>
public class SampleDbContext : CoordinatorDbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add custom entity configurations here if needed
        // modelBuilder.Entity<MyCustomEntity>()...
    }
}