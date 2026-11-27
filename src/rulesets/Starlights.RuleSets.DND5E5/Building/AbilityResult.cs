namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// A recorded ability input and independently calculated score and modifier.
/// </summary>
public sealed record AbilityResult(string Id, string Name, int? Input, int? Score, int? Modifier,
    int? OrdinaryScore, int Maximum, bool IsFallback, string? Issue, IReadOnlyList<string> Sources);
