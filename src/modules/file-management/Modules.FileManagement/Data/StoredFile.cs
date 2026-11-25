namespace Starlights.Modules.FileManagement.Data;

internal sealed class StoredFile(Guid id, Guid ownerId, string fileName, string contentType, long sizeBytes, DateTimeOffset createdAt)
{
    public Guid Id { get; private set; } = id;
    public Guid OwnerId { get; private set; } = ownerId;
    public string FileName { get; private set; } = fileName;
    public string ContentType { get; private set; } = contentType;
    public long SizeBytes { get; private set; } = sizeBytes;
    public DateTimeOffset CreatedAt { get; private set; } = createdAt;
    public bool IsDeleting { get; private set; }

    public void BeginDeletion() => IsDeleting = true;
}
