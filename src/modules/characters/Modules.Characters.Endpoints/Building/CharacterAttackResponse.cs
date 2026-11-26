namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>An equipped weapon's attack-ready profile; it does not execute an attack.</summary>
public sealed record CharacterAttackResponse(string Name, int ToHit, string Damage, string? Mastery);
