namespace Starlights.Modules.Characters.Data;

/// <summary>Ruleset-neutral saved Character metadata, including retained legacy entries.</summary>
public sealed record CharacterHeader(Guid CharacterId, string Name, string? RulesetId)
{
    /// <summary>Gets the exact saved binding without changing the retained three-argument summary constructor.</summary>
    public Starlights.Modules.Characters.Domain.Building.CharacterContentBinding? Content { get; init; }
}
