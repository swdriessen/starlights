using System.Globalization;

namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents an abbreviation component for an element.
/// </summary>
public sealed class AbbreviationComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbbreviationComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The element this component belongs to.</param>
    /// <param name="abbreviation">The abbreviation value.</param>
    public AbbreviationComponent(ElementId owningElement, string abbreviation)
        : base(owningElement)
    {
        UpdateAbbreviation(abbreviation);
    }

    /// <summary>
    /// Gets the abbreviation value, always trimmed and uppercase.
    /// </summary>
    public string Abbreviation { get; private set; } = string.Empty;

    /// <summary>
    /// Updates the abbreviation value, enforcing trim and uppercase.
    /// </summary>
    /// <param name="abbreviation">The new abbreviation value.</param>
    public void UpdateAbbreviation(string abbreviation)
    {
        if (string.IsNullOrWhiteSpace(abbreviation))
        {
            throw new ArgumentException("Abbreviation cannot be null or whitespace.", nameof(abbreviation));
        }
        Abbreviation = abbreviation.Trim().ToUpper(CultureInfo.InvariantCulture);
    }
}
