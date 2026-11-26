namespace Starlights.RuleSets.DND5E5;

/// <summary>Identifies a package item directly or through its explicitly named decision.</summary>
public sealed record DndPackageItem(string? Target, string? TargetSelection, int Quantity);
