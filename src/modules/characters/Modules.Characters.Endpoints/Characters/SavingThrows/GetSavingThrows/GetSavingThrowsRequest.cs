using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Characters.SavingThrows.GetSavingThrows;

internal sealed class GetSavingThrowsRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}
