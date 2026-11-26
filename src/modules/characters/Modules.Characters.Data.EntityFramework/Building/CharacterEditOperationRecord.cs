using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterEditOperationRecord
{
    public CharacterId CharacterId { get; set; }
    public Guid OperationId { get; set; }
    public long Revision { get; set; }
    public string Fingerprint { get; set; } = "";
    public string Status { get; set; } = "pending";
    public string? Failure { get; set; }
    public string? Sheet { get; set; }
}
