namespace Starlights.RuleSets.DND5E5;

/// <summary>Identifies the specific weapon definition for which a mastery was chosen.</summary>
public sealed record DndWeaponMasteryProperty(string WeaponId) : IDndElementProperty;
