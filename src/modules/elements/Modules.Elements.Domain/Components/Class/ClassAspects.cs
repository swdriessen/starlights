namespace Starlights.Modules.Elements.Domain.Components.Class;

public sealed class ClassAspects : ElementComponentBase
{
    public ClassAspects(ElementId owningElement, HitPointDie hitDice)
        : base(owningElement)
    {
        HitDice = hitDice;
    }

    /// <summary>
    /// Gets the hit dice associated with the class.
    /// </summary>
    public HitPointDie HitDice { get; }
}
