var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights-db", "starlights-db");

var application = builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

if (builder.ExecutionContext.IsRunMode)
{
    // run migrations in development environment to ensure the database is up-to-date
    var elementsMigrationService = builder.AddProject<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-migrations")
        .WithReference(database)
        .WithParentRelationship(database)
        .WaitFor(database);

    var charactersMigrationService = builder.AddProject<Projects.Modules_Characters_Data_EntityFramework_MigrationService>("characters-migrations")
        .WithReference(database)
        .WithParentRelationship(database)
        .WaitFor(database);

    // wait for the migration services to complete before starting the application
    application.WaitForCompletion(elementsMigrationService);
    application.WaitForCompletion(charactersMigrationService);
}

builder.Build().Run();
