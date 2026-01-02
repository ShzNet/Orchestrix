var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("orchestrixdb");

builder.AddProject<Projects.Orchestrix_Coordinator_Sample>("Coordinator")
    .WaitFor(redis)
    .WaitFor(db)
    .WithReference(redis)
    .WithReference(db)
    .WithReplicas(2);

builder.AddProject<Projects.Orchestrix_Worker_Sample>("Worker")
    .WaitFor(redis)
    .WithReference(redis)
    .WithReplicas(2);

builder.Build().Run();