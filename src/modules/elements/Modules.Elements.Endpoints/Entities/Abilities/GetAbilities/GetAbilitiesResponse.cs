using Starlights.Modules.Elements.Integration.Models;

namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.GetAbilities;

public record GetAbilitiesResponse
{
    public List<AbilityDataModel> Abilities { get; init; } = [];
}
