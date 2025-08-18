namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for saving throw information. This is used when creating a character.
/// </summary>
public record SavingThrowDataModel
{
    public SavingThrowDataModel(Guid elementId, string name, Guid primaryAbilityElementId)
    {
        ElementId = elementId;
        Name = name;
        PrimaryAbilityElementId = primaryAbilityElementId;
    }
    public Guid ElementId { get; init; }
    public string Name { get; init; }
    public Guid PrimaryAbilityElementId { get; init; }
}