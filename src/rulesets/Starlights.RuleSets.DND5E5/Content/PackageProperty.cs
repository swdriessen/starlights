namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Defines an acquisition package; choosing it does not acquire its contents.
/// </summary>
public sealed record PackageProperty(int Gold, IReadOnlyList<PackageItem> Items) : IElementProperty;
