var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Session)
    .AddDatabase("starlights-db", "starlights-db");

var application = builder.AddProject<Projects.Starlights_Application>("starlights-application")
    .WithReference(database)
    .WaitFor(database);

if (builder.ExecutionContext.IsRunMode)
{
    var migrationResource = builder.AddMigrationsResource("migrations");

    var elementsMigrationService = migrationResource.WithMigrationWorker<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-context")
        .WithReference(database)
        .WaitFor(database);

    var charactersMigrationService = migrationResource.WithMigrationWorker<Projects.Modules_Characters_Data_EntityFramework_MigrationService>("characters-context")
        .WithReference(database)
        .WaitFor(database);

    // wait for the migration services to complete before starting the application
    application.WaitForCompletion(elementsMigrationService);
    application.WaitForCompletion(charactersMigrationService);
}

builder.Build().Run();
