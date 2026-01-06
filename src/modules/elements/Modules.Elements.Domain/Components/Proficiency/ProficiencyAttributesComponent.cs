namespace Starlights.Modules.Elements.Domain.Components.Proficiency;

public class ProficiencyAttributesComponent : ElementComponentBase
{
    public ProficiencyAttributesComponent(ElementId owningElement, string proficiencyType)
        : base(owningElement)
    {
        ProficiencyType = proficiencyType;
    }

    /// <summary>
    /// Gets the type of proficiency. (e.g. "Skill", "Saving Throw", "Weapon", "Armor", "Tool")
    /// </summary>
    public string ProficiencyType { get; private set; }

    /// <summary>
    /// Updates the type of proficiency.
    /// </summary>
    public void UpdateProficiencyType(string proficiencyType)
    {
        if (proficiencyType == ProficiencyType)
        {
            return;
        }

        ProficiencyType = proficiencyType;
    }
}