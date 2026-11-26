using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterBuildRecord
{
    public CharacterId CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public required string PackId { get; set; }
    public required string PackVersion { get; set; }
    public required string CharacterElementId { get; set; }
    public required string Composition { get; set; }
    public long Revision { get; set; }
    public string Status { get; set; } = "ready";
    public Guid? OperationId { get; set; }
    public string? Failure { get; set; }
    public string? Sheet { get; set; }
    public long? SheetRevision { get; set; }
}
