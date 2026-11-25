namespace Starlights.Modules.Characters.Data;

/// <summary>Ruleset-neutral saved Character metadata, including retained legacy entries.</summary>
public sealed record CharacterHeader(Guid CharacterId, string Name, string? RulesetId);
