namespace Starlights.RuleSets.DND5E5;

/// <summary>Defines an acquisition package; choosing it does not acquire its contents.</summary>
public sealed record DndPackageProperty(int Gold, IReadOnlyList<DndPackageItem> Items) : IDndElementProperty;
