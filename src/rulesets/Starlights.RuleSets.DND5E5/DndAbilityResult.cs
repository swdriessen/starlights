namespace Starlights.RuleSets.DND5E5;

/// <summary>A recorded ability input and independently calculated score and modifier.</summary>
public sealed record DndAbilityResult(string Id, string Name, int? Input, int? Score, int? Modifier);
