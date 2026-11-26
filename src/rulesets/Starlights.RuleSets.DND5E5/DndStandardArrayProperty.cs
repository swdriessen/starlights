namespace Starlights.RuleSets.DND5E5;

/// <summary>Defines the supported standard-array input method.</summary>
public sealed record DndStandardArrayProperty(IReadOnlyList<int> Values) : IDndElementProperty;
