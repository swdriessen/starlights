namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Defines the supported standard-array input method.
/// </summary>
public sealed record StandardArrayProperty(IReadOnlyList<int> Values) : IElementProperty;
