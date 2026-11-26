namespace Starlights.RuleSets.DND5E5;

/// <summary>An attack-ready profile derived from its owned equipped weapon.</summary>
public sealed record DndAttack(string Name, int ToHit, string Damage, string? Mastery);
