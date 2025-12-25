using Starlights.Modules.Elements.Domain.Values;

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
    public AbbreviationComponent(ElementId owningElement, Abbreviation abbreviation)
        : base(owningElement)
    {
        Abbreviation = abbreviation;
    }

    /// <summary>
    /// Gets the abbreviation value, always trimmed and uppercase.
    /// </summary>
    public Abbreviation Abbreviation { get; private set; }

    /// <summary>
    /// Updates the abbreviation value.
    /// </summary>
    public void UpdateAbbreviation(Abbreviation abbreviation)
    {
        Abbreviation = abbreviation;
    }
}
