namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Initializes a new instance of the <see cref="AbilityComponent"/> class.
/// </summary>
public sealed class AbilityComponent : ElementComponentBase
{
    public AbilityComponent(string abbreviation)
    {
        Abbreviation = abbreviation.Trim();
    }

    /// <summary>
    /// Gets the abbreviation of the ability component.
    /// </summary>
    public string Abbreviation { get; private set; }

    /// <summary>
    /// Updates the abbreviation of the ability component.
    /// </summary>
    public void UpdateAbbreviation(string abbreviation)
    {
        if (string.IsNullOrWhiteSpace(abbreviation))
        {
            throw new ArgumentException("Abbreviation cannot be null or whitespace.", nameof(abbreviation));
        }

        Abbreviation = abbreviation.Trim();
    }
}