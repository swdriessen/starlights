using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Spellcasting;

[DebuggerDisplay("Time = {Value}, Ritual = {IsRitual}")]
public readonly record struct CastingTime
{
    public CastingTime(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
    }

    /// <summary>
    /// Gets the casting time value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Indicates whether the spell can be cast as a ritual.
    /// </summary>
    public bool IsRitual { get; init; }
}
