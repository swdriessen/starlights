namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// Evidence of a particular starting-package acquisition, retained when choices change.
/// </summary>
public sealed record EquipmentReceipt
{

    /// <summary>
    /// Copies the acquired items so callers cannot change possessions outside the root.
    /// </summary>
    public EquipmentReceipt(string selectionId, string optionId, int gold, IReadOnlyList<OwnedItem> items)
    {
        SelectionId = selectionId;
        OptionId = optionId;
        Gold = gold;
        Items = Array.AsReadOnly(items.ToArray());
    }

    /// <summary>
    /// Gets the originating decision identity.
    /// </summary>
    public string SelectionId
    {
        get;
    }

    /// <summary>
    /// Gets the acquired package definition.
    /// </summary>
    public string OptionId
    {
        get;
    }

    /// <summary>
    /// Gets the acquired currency.
    /// </summary>
    public int Gold
    {
        get;
    }

    /// <summary>
    /// Gets independently identified possessions.
    /// </summary>
    public IReadOnlyList<OwnedItem> Items
    {
        get;
    }
}
