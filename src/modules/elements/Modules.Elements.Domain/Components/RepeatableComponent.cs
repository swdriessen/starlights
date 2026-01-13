namespace Starlights.Modules.Elements.Domain.Components;

public class RepeatableComponent : ElementComponentBase
{
    public RepeatableComponent(ElementId owningElement, bool isRepeatable = false)
        : base(owningElement)
    {
        IsRepeatable = isRepeatable;
    }

    /// <summary>
    /// Gets a value indicating whether the element is repeatable. When true, the element can be included multiple times.
    /// </summary>
    public bool IsRepeatable { get; private set; }

    /// <summary>
    /// Sets whether the element is repeatable.
    /// </summary>
    public void SetRepeatable(bool isRepeatable)
    {
        if (IsRepeatable == isRepeatable)
        {
            return;
        }

        IsRepeatable = isRepeatable;
    }
}