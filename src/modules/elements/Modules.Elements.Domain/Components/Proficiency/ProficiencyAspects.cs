namespace Starlights.Modules.Elements.Domain.Components.Proficiency;

public class ProficiencyAspects : ElementComponentBase
{
    public ProficiencyAspects(ElementId owningElement, ProficiencyClassification classification)
        : base(owningElement)
    {
        Classification = classification;
    }

    /// <summary>
    /// Gets the type of proficiency. (e.g. "Skill", "Saving Throw", "Weapon", "Armor", "Tool")
    /// </summary>
    public ProficiencyClassification Classification { get; private set; }

    /// <summary>
    /// Updates the type of proficiency.
    /// </summary>
    public void UpdateClassification(ProficiencyClassification classification)
    {
        if (classification == Classification)
        {
            return;
        }

        Classification = classification;
    }
}
