using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.ServiceDefaults;
using Orchestrix.Transport;
using Orchestrix.Transport.Redis;
using Orchestrix.Transport.Serialization;
using Serilog;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console());

builder.AddServiceDefaults();

// Configure Redis Transport
var redisConnectionString = builder.Configuration.GetConnectionString("redis") ?? "localhost:6379";
builder.Services.TryAddSingleton<RedisTransportOptions>();
builder.Services.TryAddSingleton<IMessageSerializer, JsonMessageSerializer>();
builder.Services.TryAddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<IPublisher, RedisPublisher>();
builder.Services.AddSingleton<Orchestrix.Transport.ISubscriber, RedisSubscriber>();
builder.Services.AddSingleton<TransportChannels>();

// Configure Worker
builder.Services.AddOrchestrixWorker(
    options =>
    {
        options.Queues = ["default", "high-priority"];
        options.MaxConcurrentJobs = 5;
    },
    workerBuilder =>
    {
        // Register job handlers
        workerBuilder.AddJobHandler<SampleJobHandler, SampleJob>(nameof(SampleJob));
        workerBuilder.AddJobHandler<LongRunningJobHandler, LongRunningJob>(nameof(LongRunningJob));
    });

var host = builder.Build();

try
{
    Log.Information("Starting Orchestrix Worker Sample...");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
