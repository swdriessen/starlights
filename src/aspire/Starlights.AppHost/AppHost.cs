var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights-db", "starlights-db");

IResourceBuilder<ProjectResource> elementsMigrationService = null!;
IResourceBuilder<ProjectResource> charactersMigrationService = null!;

if (builder.ExecutionContext.IsRunMode)
{
    // run migrations in development environment to ensure the database is up-to-date
    elementsMigrationService = builder.AddProject<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-migrations")
        .WithReference(database)
        .WaitFor(database);

    charactersMigrationService = builder.AddProject<Projects.Modules_Characters_Data_EntityFramework_MigrationService>("characters-migrations")
        .WithReference(database)
        .WaitFor(database);
}

var application = builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

if (builder.ExecutionContext.IsRunMode)
{
    // wait for the migration services to complete before starting the application
    application.WaitForCompletion(elementsMigrationService);
    application.WaitForCompletion(charactersMigrationService);
}

builder.Build().Run();
