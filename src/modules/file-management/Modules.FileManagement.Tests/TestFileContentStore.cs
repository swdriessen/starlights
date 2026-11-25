using System.Collections.Concurrent;
using Starlights.Modules.FileManagement.Storage;

namespace Starlights.Modules.FileManagement.Tests;

internal sealed class TestFileContentStore : IFileContentStore
{
    private readonly ConcurrentDictionary<Guid, byte[]> _files = [];
    public int Count => _files.Count;
    public bool FailNextDeletion { get; set; }

    public Task UploadAsync(Guid id, byte[] bytes, string contentType, CancellationToken cancellationToken)
    {
        _files[id] = bytes.ToArray();
        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult<Stream>(new MemoryStream(_files[id], writable: false));

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (FailNextDeletion)
        {
            FailNextDeletion = false;
            throw new IOException("Simulated storage outage.");
        }
        _files.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
