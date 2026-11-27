using System.Text.Json.Serialization;

namespace Starlights.RuleSets.DND5E5;

/// <summary>A retained four-die result whose total drops exactly one lowest die.</summary>
public sealed record DndAbilityRoll
{
    /// <summary>Copies and validates the original dice without generating new values.</summary>
    [JsonConstructor]
    public DndAbilityRoll(IReadOnlyList<int> dice)
    {
        if (dice.Count != 4 || dice.Any(die => die is < 1 or > 6))
            throw new ArgumentException("A generated result requires four six-sided dice.", nameof(dice));
        Dice = Array.AsReadOnly(dice.ToArray());
    }

    /// <summary>Gets the original dice in rolling order.</summary>
    public IReadOnlyList<int> Dice { get; }
    /// <summary>Gets the sum after dropping one lowest die.</summary>
    public int Total => Dice.Sum() - Dice.Min();
}
