namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for skill information. This is used when creating a character.
/// </summary>
public record SkillDataModel
{
    public SkillDataModel(Guid elementId, string name, Guid primaryAbilityElementId)
    {
        ElementId = elementId;
        Name = name;
        PrimaryAbilityElementId = primaryAbilityElementId;
    }
    public Guid ElementId { get; init; }
    public string Name { get; init; }
    public Guid PrimaryAbilityElementId { get; init; }
    public double SortingOrder { get; init; }
}
