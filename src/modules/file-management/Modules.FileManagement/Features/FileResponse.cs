using System.ComponentModel.DataAnnotations;

namespace Starlights.Modules.FileManagement.Features;

/// <summary>Describes a private file without exposing its storage location.</summary>
public sealed record FileResponse(
    [property: Required] Guid Id,
    [property: Required] string FileName,
    [property: Required] string ContentType,
    [property: Required] long SizeBytes,
    [property: Required] DateTimeOffset CreatedAt,
    [property: Required] string ContentUrl,
    [property: Required] bool IsDeleting);
