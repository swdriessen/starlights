namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Identifies the specific weapon definition for which a mastery was chosen.
/// </summary>
public sealed record WeaponMasteryProperty(string WeaponId) : IElementProperty;
