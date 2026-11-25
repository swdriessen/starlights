using System.ComponentModel.DataAnnotations;

namespace Starlights.Modules.FileManagement.Features;

/// <summary>Uploads original file bytes using bounded base64 content in the generated JSON client.</summary>
public sealed record UploadFileRequest
{
    /// <summary>Gets the display filename; directory components are not retained.</summary>
    [Required, MaxLength(255)]
    public required string FileName { get; init; }

    /// <summary>Gets the original bytes encoded as base64, without a data-URL prefix.</summary>
    [Required, MaxLength(6_990_508)]
    public required string ContentBase64 { get; init; }
}
