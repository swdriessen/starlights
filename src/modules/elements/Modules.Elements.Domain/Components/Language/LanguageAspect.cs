namespace Starlights.Modules.Elements.Domain.Components.Language;

/// <summary>
/// Represents a component that defines the language aspect of an element.
/// </summary>
public sealed class LanguageAspect : ElementComponentBase
{
    public LanguageAspect(ElementId owningElement, LanguageClassification classification, string origin)
        : base(owningElement)
    {
        Classification = classification;
        Origin = origin.Trim();
    }

    public LanguageAspect(ElementId owningElement)
        : base(owningElement)
    {
        Classification = LanguageClassification.Standard;
        Origin = string.Empty;
    }

    /// <summary>
    /// Gets the classification of the language which indicates its kind.
    /// </summary>
    public LanguageClassification Classification { get; private set; }

    /// <summary>
    /// Gets the origin description of the language.
    /// </summary>
    public string Origin { get; private set; }

    /// <summary>
    /// Updates the classification of the language.
    /// </summary>
    public void UpdateClassification(LanguageClassification classifcation)
    {
        Classification = classifcation;
    }

    /// <summary>
    /// Updates the origin description of the language. Value can be empty but not null.
    /// </summary>
    public void UpdateOrigin(string origin)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        Origin = origin.Trim();
    }
}
