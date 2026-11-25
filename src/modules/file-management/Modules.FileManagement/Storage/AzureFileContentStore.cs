using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Starlights.Modules.FileManagement.Storage;

internal sealed class AzureFileContentStore(BlobContainerClient container) : IFileContentStore
{
    public async Task UploadAsync(Guid id, byte[] bytes, string contentType, CancellationToken cancellationToken)
    {
        using var content = new MemoryStream(bytes, writable: false);
        await container.GetBlobClient(id.ToString("N")).UploadAsync(content,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } }, cancellationToken);
    }

    public async Task<Stream> OpenReadAsync(Guid id, CancellationToken cancellationToken)
        => (await container.GetBlobClient(id.ToString("N")).DownloadStreamingAsync(cancellationToken: cancellationToken)).Value.Content;

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        => await container.GetBlobClient(id.ToString("N")).DeleteIfExistsAsync(cancellationToken: cancellationToken);
}
