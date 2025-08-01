var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights-db", "starlights-db");


if (builder.ExecutionContext.IsRunMode)
{
    builder.AddProject<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-migrationservice")
        .WithReference(database)
        .WaitFor(database);
}

builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
