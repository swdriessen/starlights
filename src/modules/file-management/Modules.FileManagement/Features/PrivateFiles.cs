using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Modules.FileManagement.Data;
using Starlights.Modules.FileManagement.Storage;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.FileManagement.Features;

internal sealed class PrivateFiles(FileManagementContext context, IFileContentStore content, ICurrentUser currentUser,
    TimeProvider clock, ILogger<PrivateFiles> logger) : IPrivateFiles
{
    internal static readonly ActivitySource ActivitySource = new("Starlights.Modules.FileManagement");
    private const int PageSize = 50;
    private Guid OwnerId => currentUser.UserId ?? throw new InvalidOperationException("Private files require an authenticated owner.");
    private IQueryable<StoredFile> OwnedFiles => context.Files.Where(file => file.OwnerId == OwnerId);

    public async Task<FileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("UploadPrivateFile");
        var owner = OwnerId;
        var fileName = Path.GetFileName(request.FileName.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Any(char.IsControl))
            throw new FileUploadException("Choose a valid filename.");
        var validated = ImageUploadValidator.Validate(request.ContentBase64);
        var file = new StoredFile(Guid.NewGuid(), owner, fileName, validated.ContentType, validated.Bytes.LongLength, clock.GetUtcNow());
        await content.UploadAsync(file.Id, validated.Bytes, file.ContentType, cancellationToken);
        context.Files.Add(file);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is DbUpdateException or OperationCanceledException)
        {
            logger.LogError(exception, "Failed to persist uploaded file {FileId}; removing its content", file.Id);
            try
            {
                await content.DeleteAsync(file.Id, CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(cleanupException, "Failed to remove uncommitted content for {FileId}; operator cleanup is required", file.Id);
                throw new AggregateException("File persistence and content cleanup both failed.", exception, cleanupException);
            }
            throw;
        }
        logger.LogInformation("Uploaded private file {FileId} for {OwnerId}, {SizeBytes} bytes", file.Id, owner, file.SizeBytes);
        return ToResponse(file);
    }

    public async Task<FileListResponse> ListAsync(int offset, CancellationToken cancellationToken)
    {
        var page = await OwnedFiles.AsNoTracking().OrderByDescending(file => file.CreatedAt).ThenBy(file => file.Id)
            .Skip(offset).Take(PageSize + 1).ToListAsync(cancellationToken);
        return new FileListResponse(page.Take(PageSize).Select(ToResponse).ToArray(),
            page.Count > PageSize ? checked(offset + PageSize) : null);
    }

    public async Task<FileResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var file = await OwnedFiles.AsNoTracking().SingleOrDefaultAsync(file => file.Id == id, cancellationToken);
        return file is null ? null : ToResponse(file);
    }

    public async Task<FileContent?> OpenReadAsync(Guid id, CancellationToken cancellationToken)
    {
        var file = await GetAsync(id, cancellationToken);
        return file is null || file.IsDeleting ? null : new FileContent(await content.OpenReadAsync(id, cancellationToken), file.ContentType);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("DeletePrivateFile");
        var file = await OwnedFiles.SingleOrDefaultAsync(file => file.Id == id, cancellationToken);
        if (file is null)
            return false;
        file.BeginDeletion();
        await context.SaveChangesAsync(cancellationToken);
        await content.DeleteAsync(id, cancellationToken);
        await OwnedFiles.Where(candidate => candidate.Id == id).ExecuteDeleteAsync(cancellationToken);
        logger.LogInformation("Deleted private file {FileId} for {OwnerId}", id, file.OwnerId);
        return true;
    }

    private static FileResponse ToResponse(StoredFile file)
        => new(file.Id, file.FileName, file.ContentType, file.SizeBytes, file.CreatedAt, $"/api/files/{file.Id}/content", file.IsDeleting);
}
