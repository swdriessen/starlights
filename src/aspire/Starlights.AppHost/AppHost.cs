var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddSqlServer("sql-server", port: 61070) // static port for now, easier to view database in SSMS
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .AddDatabase("starlights-db", "starlights-db-env");

var application = builder.AddProject<Projects.Starlights_Application>("backend")
    .WithUrls(context => context.Urls.ForEach(url => url.DisplayLocation = UrlDisplayLocation.DetailsOnly))
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
        .WaitFor(elements);

    // wait for the migration services to complete before starting the application
    application.WaitForCompletion(elements);
    application.WaitForCompletion(characters);
}

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddJavaScriptApp("app-landing-page", "../../frontend/apps/landing-page", "dev")
        .WithEnvironment("BROWSER", "none")
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(application)
        .WaitFor(application);

    builder.AddJavaScriptApp("app-character-builder", "../../frontend/apps/builder-app", "dev")
        //.WithEnvironment("BROWSER", "none")
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(application)
        .WaitFor(application);

    builder.AddJavaScriptApp("app-content-manager", "../../frontend/apps/content-manager", "dev")
        .WithEnvironment("BROWSER", "none")
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(application)
        .WaitFor(application);

    builder.AddJavaScriptApp("app-showcase", "../../frontend/apps/showcase", "dev")
        .WithEnvironment("BROWSER", "none")
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(application)
        .WaitFor(application);
}

if (builder.ExecutionContext.IsRunMode)
{
    // this section allows access to the frontend from outside the local network using a developer tunnel
    // this does not start the tunnel automatically, the tunnel must be started explicitly
    // when the dev tunnel becomes available, the frontend will start automatically

    var tunnel = builder.AddDevTunnel("dev-tunnel")
        .WithAnonymousAccess()
        .WithExplicitStart();

    var app = builder.AddJavaScriptApp("dev-tunnel-react-builder-app", "../../frontend/apps/builder-app", "dev")
        .WithIconName("Cloud", IconVariant.Regular)
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(application, tunnel)
        .WithParentRelationship(tunnel)
        .WaitFor(application);

    tunnel.WithReference(application);
    tunnel.WithReference(app);
}

builder.Build().Run();