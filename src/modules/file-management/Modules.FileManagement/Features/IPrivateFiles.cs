namespace Starlights.Modules.FileManagement.Features;

internal interface IPrivateFiles
{
    Task<FileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken);
    Task<FileListResponse> ListAsync(int offset, CancellationToken cancellationToken);
    Task<FileResponse?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<FileContent?> OpenReadAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
