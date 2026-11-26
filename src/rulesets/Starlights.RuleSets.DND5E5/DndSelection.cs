namespace Starlights.RuleSets.DND5E5;

/// <summary>An occurrence-qualified decision with current eligible candidates and retained recorded answers.</summary>
public sealed record DndSelection(string Id, string Name, int Count, IReadOnlyList<string> Answers,
    IReadOnlyList<DndContentElement> Options, bool IsPackage);
