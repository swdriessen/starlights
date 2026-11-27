namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Identifies a package item directly or through its explicitly named decision.
/// </summary>
public sealed record PackageItem(string? Target, string? TargetSelection, int Quantity);
