namespace Starlights.Modules.FileManagement.Storage;

internal interface IFileContentStore
{
    Task UploadAsync(Guid id, byte[] bytes, string contentType, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
