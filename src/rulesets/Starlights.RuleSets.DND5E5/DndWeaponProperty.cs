namespace Starlights.RuleSets.DND5E5;

/// <summary>Defines the supported weapon profile, separately from possession and mastery intent.</summary>
public sealed record DndWeaponProperty(string Ability, string Damage, string DamageType, string Mastery) : IDndElementProperty;
