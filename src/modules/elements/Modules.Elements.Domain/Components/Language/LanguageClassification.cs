using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Language;

[DebuggerDisplay("{Value}")]
public readonly record struct LanguageClassification
{
    public static readonly LanguageClassification Standard = new("Standard");
    public static readonly LanguageClassification Rare = new("Rare");

    public LanguageClassification(string kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        Kind = kind.Trim();
    }

    /// <summary>
    /// Gets the kind of language. E.g., "Standard" or "Rare".
    /// </summary>
    public string Kind { get; }

    public static implicit operator string(LanguageClassification kind)
    {
        return kind.Kind;
    }
}