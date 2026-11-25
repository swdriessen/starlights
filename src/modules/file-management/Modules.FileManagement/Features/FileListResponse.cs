using System.ComponentModel.DataAnnotations;

namespace Starlights.Modules.FileManagement.Features;

/// <summary>Returns a bounded page of the owner's private files and the next offset, when present.</summary>
public sealed record FileListResponse([property: Required] IReadOnlyList<FileResponse> Files, int? NextOffset);
