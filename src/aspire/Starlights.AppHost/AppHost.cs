var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("starlights-db")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights");

builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
