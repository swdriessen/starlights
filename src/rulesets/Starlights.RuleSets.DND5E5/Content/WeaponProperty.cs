namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Defines the supported weapon profile, separately from possession and mastery intent.
/// </summary>
public sealed record WeaponProperty(string Ability, string Damage, string DamageType, string Mastery) : IElementProperty;
