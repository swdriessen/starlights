using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Class;

/// <summary>
/// Represents the hit dice used for determining hit points in a class.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record HitPointDie
{
    public static HitPointDie D4 => new(4);
    public static HitPointDie D6 => new(6);
    public static HitPointDie D8 => new(8);
    public static HitPointDie D10 => new(10);
    public static HitPointDie D12 => new(12);

    public HitPointDie(int size, int amount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Size = size;
        Amount = amount;
    }

    /// <summary>
    /// Gets the size of the hit dice (e.g., 6 for a d6).
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the amount of hit dice. Default is 1.
    /// </summary>
    public int Amount { get; } = 1;

    public override string ToString()
    {
        return Amount > 1 ? $"{Amount}d{Size}" : $"d{Size}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}