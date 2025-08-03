#pragma warning disable IDE0130 // Namespace does not match folder structure: aspire convention, avoid namespaces in apphost.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aspire.Hosting;

public sealed class MigrationsResource : Resource
{
    public MigrationsResource(string name)
        : base(name)
    {

    }
}

public static class MigrationsResourceExtensions
{
    /// <summary>
    /// Adds a migrations resource to the distributed application builder.
    /// </summary>
    public static IResourceBuilder<MigrationsResource> AddMigrationsResource(this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new MigrationsResource(name);
        var resourceBuilder = builder.AddResource(resource);

        // set state to "Migrating"

        builder.Eventing.Subscribe<BeforeStartEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("================= BeforeStartEvent =================");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("================= AfterResourcesCreatedEvent =================");

                return Task.CompletedTask;
            });



        resourceBuilder.OnInitializeResource(
            static (resource, @event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("1. OnInitializeResource {ResourceName}", resource.Name);

                return Task.CompletedTask;
            });
        resourceBuilder.OnResourceReady(
            static (resource, @event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("2. OnResourceReady {ResourceName}", resource.Name);

                return Task.CompletedTask;
            });


        return resourceBuilder;
    }

    /// <summary>
    /// Adds a migration worker project to the migrations resource.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithMigrationWorker<TProject>(this IResourceBuilder<MigrationsResource> migrationBuilder, string name)
        where TProject : IProjectMetadata, new()
    {
        return migrationBuilder.ApplicationBuilder.AddProject<TProject>(name)
            .WithParentRelationship(migrationBuilder.Resource);
    }
}