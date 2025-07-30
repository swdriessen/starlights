namespace Starlights.Modules.Elements.Integration.Abstractions.Models;

/// <summary>
/// The DTO model for ability information. This is used when creating a character.
/// </summary>
public record AbilityInfo
{
    public AbilityInfo(Guid elementId, string name, string abbreviation)
    {
        ElementId = elementId;
        Name = name;
        Abbreviation = abbreviation;
    }

    public Guid ElementId { get; init; }
    public string Name { get; init; }
    public string Abbreviation { get; init; }
}