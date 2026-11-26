using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Starlights.RuleSets.DND5E5;

/// <summary>Immutable player facts, independent of the recalculable sheet.</summary>
public sealed record DndBuildFacts
{
    /// <summary>Copies persisted facts into protected read-only views.</summary>
    [JsonConstructor]
    public DndBuildFacts(IReadOnlyDictionary<string, IReadOnlyList<string>> choices,
        IReadOnlyDictionary<string, int> inputs, IReadOnlyList<DndEquipmentReceipt> receipts,
        string? armorId, string? weaponId)
    {
        Choices = new ReadOnlyDictionary<string, IReadOnlyList<string>>(choices.ToDictionary(
            pair => pair.Key, pair => (IReadOnlyList<string>)Array.AsReadOnly(pair.Value.ToArray()), StringComparer.Ordinal));
        Inputs = new ReadOnlyDictionary<string, int>(inputs.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal));
        Receipts = Array.AsReadOnly(receipts.ToArray());
        ArmorId = armorId;
        WeaponId = weaponId;
    }
    /// <summary>Gets recorded intent, including decisions whose origin is no longer active.</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Choices { get; }
    /// <summary>Gets player-assigned base ability values.</summary>
    public IReadOnlyDictionary<string, int> Inputs { get; }
    /// <summary>Gets independent, irreversible starting-package receipts.</summary>
    public IReadOnlyList<DndEquipmentReceipt> Receipts { get; }
    /// <summary>Gets the owned armor allocation.</summary>
    public string? ArmorId { get; }
    /// <summary>Gets the owned weapon allocation.</summary>
    public string? WeaponId { get; }
    /// <summary>Creates a new uncompleted build.</summary>
    public static DndBuildFacts Empty => new(new Dictionary<string, IReadOnlyList<string>>(), new Dictionary<string, int>(), [], null, null);
}
