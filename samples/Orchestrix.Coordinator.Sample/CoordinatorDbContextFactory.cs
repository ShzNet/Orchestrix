using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Orchestrix.Persistence.EfCore;

/// <summary>
/// Design-time factory for creating <see cref="CoordinatorDbContext"/> to enable EF Core migrations.
/// </summary>
public class CoordinatorDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
{
    public SampleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();

        // Use a dummy connection string for design-time. 
        // The actual connection string at runtime comes from configuration.
        // We configure the migrations assembly to be this sample project.
        optionsBuilder.UseNpgsql("Host=localhost;Database=orchestrix_db;Username=postgres;Password=postgres", b =>
        {
            b.MigrationsAssembly("Orchestrix.Coordinator.Sample");
        });

        return new SampleDbContext(optionsBuilder.Options);
    }
}