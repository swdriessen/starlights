using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Domain.Components.Ability;

public sealed class AbilityAspects : ElementComponentBase
{
    public AbilityAspects(ElementId owningElement, Abbreviation abbreviation)
        : base(owningElement)
    {
        Abbreviation = abbreviation;
    }

    /// <summary>
    /// Gets the abbreviation of the ability name.
    /// </summary>
    public Abbreviation Abbreviation { get; private set; }

    /// <summary>
    /// Updates the abbreviation of the ability component.
    /// </summary>
    public void UpdateAbbreviation(Abbreviation abbreviation)
    {
        Abbreviation = abbreviation;
    }
}