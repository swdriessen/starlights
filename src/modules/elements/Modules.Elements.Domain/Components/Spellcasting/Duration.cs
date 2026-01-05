using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Spellcasting;

[DebuggerDisplay("Duration = {Value}")]
public readonly record struct Duration
{
    public static Duration Instantaneous => new("Instantaneous");

    public static Duration Concentration(string maxDuration)
    {
        return new Duration(maxDuration, true);
    }

    public Duration(string value, bool isConcentration = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
        IsConcentration = isConcentration;
    }

    /// <summary>
    /// Gets the duration value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Indicates whether the duration requires concentration.
    /// </summary>
    public bool IsConcentration { get; init; }
}
