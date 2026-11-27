namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// An owned homogeneous stack with its pinned definition and acquisition origin.
/// </summary>
public sealed record OwnedItem(string Id, string ElementId, int Quantity);
