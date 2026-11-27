using Starlights.RuleSets.DND5E5.Content;

namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// An occurrence-qualified decision with current eligible candidates and retained recorded answers.
/// </summary>
public sealed record Selection(string Id, string Name, int Count, IReadOnlyList<string> Answers,
    IReadOnlyList<ContentElement> Options, bool IsPackage);
