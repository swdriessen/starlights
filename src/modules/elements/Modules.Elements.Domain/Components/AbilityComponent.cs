
using System.Globalization;
namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Initializes a new instance of the <see cref="AbilityComponent"/> class.
/// </summary>
public sealed class AbilityComponent : ElementComponentBase
{
    public AbilityComponent(Guid owningElement, string abbreviation)
        : base(owningElement)
    {
        UpdateAbbreviation(abbreviation);
    }

    /// <summary>
    /// Gets the abbreviation of the ability component.
    /// </summary>
    public string Abbreviation { get; private set; } = string.Empty;

    /// <summary>
    /// Updates the abbreviation of the ability component.
    /// </summary>
    public void UpdateAbbreviation(string abbreviation)
    {
        if (string.IsNullOrWhiteSpace(abbreviation))
        {
            throw new ArgumentException("Abbreviation cannot be null or whitespace.", nameof(abbreviation));
        }

        Abbreviation = abbreviation.Trim().ToUpper(CultureInfo.InvariantCulture);
    }
}