namespace Starlights.Modules.Elements.Domain;

public sealed class AbilityComponent : ElementComponentBase
{
    public AbilityComponent(string abbreviation)
    {
        Abbreviation = abbreviation.Trim();
    }

    /// <summary>
    /// Gets the abbreviation of the ability component.
    /// </summary>
    public string Abbreviation { get; }
}