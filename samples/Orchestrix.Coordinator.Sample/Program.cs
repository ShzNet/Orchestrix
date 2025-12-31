using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Locking;
using Orchestrix.Transport;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOrchestrixCoordinator(coordinator => 
{
     coordinator.Options.NodeId = "sample-node-1";
     coordinator.Locking.UseRedis(options => 
     {
         options.ConnectionString = builder.Configuration.GetConnectionString("redis") ?? "localhost:6379";
     });
     
     coordinator.Transport.UseRedis(builder.Configuration.GetConnectionString("redis") ?? "localhost:6379");
     
     coordinator.Persistence.UseEfCore(options =>
     {
         options.UseNpgsql(builder.Configuration.GetConnectionString("orchestrixdb"), b =>
         {
             b.MigrationsAssembly("Orchestrix.Coordinator.Sample");
         });
     });
});

var host = builder.Build();

// Auto-migration for Sample Project
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Orchestrix.Coordinator.Persistence.EfCore.CoordinatorDbContext>();
    await context.Database.MigrateAsync();
}

host.Run();
