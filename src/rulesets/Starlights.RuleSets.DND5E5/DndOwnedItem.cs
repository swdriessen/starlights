namespace Starlights.RuleSets.DND5E5;

/// <summary>An owned homogeneous stack with its pinned definition and acquisition origin.</summary>
public sealed record DndOwnedItem(string Id, string ElementId, int Quantity);
