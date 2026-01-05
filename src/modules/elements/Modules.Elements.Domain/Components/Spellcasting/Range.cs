using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Spellcasting;

/// <summary>
/// Represents the spellcasting range.
/// </summary>
[DebuggerDisplay("Range = {Type}, Distance = {Distance} feet")]
public readonly record struct Range
{
    public static Range Touch => new("Touch");
    public static Range Self => new("Self");
    public static Range Ranged(int distance)
    {
        return new("Ranged", distance);
    }

    public Range(string type, int? distance = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type, nameof(type));

        Type = type.Trim();

        if (distance.HasValue && distance < 0)
        {
            throw new ArgumentException("Distance cannot be negative.", nameof(distance));
        }

        Distance = distance;
    }

    /// <summary>
    /// Gets the spellcasting range type.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Gets the distance in feet if applicable.
    /// </summary>
    /// <remarks>
    /// A distance only applies for certain range types, such as "Ranged".
    /// </remarks>
    public int? Distance { get; }

    public override string ToString()
    {
        if (Distance.HasValue)
        {
            return $"{Type} ({Distance} feet)";
        }

        return Type;
    }
}
