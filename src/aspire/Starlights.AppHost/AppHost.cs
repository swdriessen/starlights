var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights-db", "starlights-db");

IResourceBuilder<ProjectResource> migrationService = null!;

if (builder.ExecutionContext.IsRunMode)
{
    // run migrations in development environment to ensure the database is up-to-date
    migrationService = builder.AddProject<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-migrations")
        .WithReference(database)
        .WaitFor(database);
}

var application = builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

if (builder.ExecutionContext.IsRunMode && migrationService is not null)
{
    // wait for the migration service to complete before starting the application
    application.WaitForCompletion(migrationService);
}

builder.Build().Run();
