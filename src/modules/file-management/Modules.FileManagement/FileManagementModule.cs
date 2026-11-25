using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.FileManagement.Data;
using Starlights.Modules.FileManagement.Features;
using Starlights.Modules.FileManagement.Middleware;
using Starlights.Modules.FileManagement.Storage;

namespace Starlights.Modules.FileManagement;

/// <summary>Registers private file ownership, persistence and storage without exposing blob implementation details.</summary>
public static class FileManagementModule
{
    /// <summary>Registers SQL metadata and private Azure Blob Storage using service-discovery connections.</summary>
    public static void AddFileManagementModule(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<FileManagementContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("starlights-db")
                ?? throw new InvalidOperationException("FileManagement requires the starlights-db connection."),
            sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "file_management").EnableRetryOnFailure()));
        builder.Services.AddSingleton(_ => new BlobServiceClient(
            builder.Configuration.GetConnectionString("files")
                ?? throw new InvalidOperationException("FileManagement requires the files storage connection."))
            .GetBlobContainerClient("private-files"));
        builder.Services.AddSingleton<IFileContentStore, AzureFileContentStore>();
        builder.Services.AddScoped<IPrivateFiles, PrivateFiles>();
        builder.Services.TryAddSingleton(TimeProvider.System);
        builder.Services.AddExceptionHandler<FileUploadExceptionHandler>();
    }

    /// <summary>Maps authenticated Minimal API endpoints and bounds upload request bodies before deserialization.</summary>
    public static void MapFileManagement(this WebApplication app)
    {
        app.UseMiddleware<FileUploadLimitMiddleware>();
        app.MapPrivateFiles();
    }

    /// <summary>Migrates metadata and initializes the private container, surfacing setup failures.</summary>
    public static async Task InitializeFileManagementAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<FileManagementContext>().Database.MigrateAsync(cancellationToken);
        var container = scope.ServiceProvider.GetRequiredService<BlobContainerClient>();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        if ((await container.GetPropertiesAsync(cancellationToken: cancellationToken)).Value.PublicAccess != PublicAccessType.None)
            throw new InvalidOperationException("The file container must not allow public access.");
    }
}
