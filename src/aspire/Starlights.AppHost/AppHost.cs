var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sql-server")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("starlights-db", "starlights-db-env");

var application = builder.AddProject<Projects.Starlights_Application>("backend")
    .WithScalarCommand()
    .WithScalarUrl()
    .WithInitializeDatabaseCommand()
    .WithReference(database)
    .WaitFor(database);

if (builder.ExecutionContext.IsRunMode)
{
    var migrationResource = builder.AddMigrationsResource("migrations");

    var elements = migrationResource.WithMigrationWorker<Projects.Modules_Elements_Data_EntityFramework_MigrationService>("elements-context")
        .WithReference(database)
        .WaitFor(database);

    var characters = migrationResource.WithMigrationWorker<Projects.Modules_Characters_Data_EntityFramework_MigrationService>("characters-context")
        .WithReference(database)
        .WaitFor(database);

    // wait for the migration services to complete before starting the application
    application.WaitForCompletion(elements);
    application.WaitForCompletion(characters);
}

builder.Build().Run();

