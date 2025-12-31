var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("orchestrixdb");

builder.AddProject<Projects.Orchestrix_Coordinator_Sample>("Orchestrix")
    .WaitFor(redis)
    .WaitFor(db)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(db)
    .WithReference(redis)
    .WithReference(db)
    .WithReplicas(2);

builder.Build().Run();