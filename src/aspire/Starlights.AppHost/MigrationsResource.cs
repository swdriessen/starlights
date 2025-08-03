#pragma warning disable IDE0130 // Namespace does not match folder structure: aspire convention, avoid namespaces in apphost.cs
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

        return resourceBuilder;
    }

    /// <summary>
    /// Adds a migration worker project to the migrations resource.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithMigrationWorker<TProject>(this IResourceBuilder<MigrationsResource> migrationBuilder, string name)
        where TProject : IProjectMetadata, new()
    {
        var projectBuilder = migrationBuilder.ApplicationBuilder.AddProject<TProject>(name)
            .WithParentRelationship(migrationBuilder.Resource);

        return projectBuilder;
    }
}