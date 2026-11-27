namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// An attack-ready profile derived from its owned equipped weapon.
/// </summary>
public sealed record Attack(string Name, int ToHit, string Damage, string? Mastery);
