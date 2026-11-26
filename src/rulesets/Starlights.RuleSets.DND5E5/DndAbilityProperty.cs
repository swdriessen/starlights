namespace Starlights.RuleSets.DND5E5;

/// <summary>Defines an input statistic and its supported modifier expression.</summary>
public sealed record DndAbilityProperty(string Prefix, string Modifier) : IDndElementProperty;
