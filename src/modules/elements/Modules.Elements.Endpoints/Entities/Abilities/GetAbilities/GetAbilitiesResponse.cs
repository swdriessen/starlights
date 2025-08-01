using Starlights.Modules.Elements.Integration.Abstractions.Models;

namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.GetAbilities;

public record GetAbilitiesResponse
{
    public List<AbilityInfo> Abilities { get; init; } = [];
}
