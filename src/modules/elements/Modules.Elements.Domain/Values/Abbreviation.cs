namespace Starlights.Modules.Elements.Domain.Values;

/// <summary>
/// Represents an abbreviation value that is always stored in uppercase and trimmed format.
/// </summary>
public readonly record struct Abbreviation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Abbreviation"/> struct which enforces a trimmed and uppercase value.
    /// </summary>
    public Abbreviation(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }

    public string Value { get; }

    public static implicit operator Abbreviation(string value)
    {
        return new Abbreviation(value);
    }

    public static implicit operator string(Abbreviation abbreviation)
    {
        return abbreviation.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
