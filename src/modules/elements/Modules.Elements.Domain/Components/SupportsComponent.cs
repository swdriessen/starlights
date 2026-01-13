namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that manages a collection of supports associated with an element.
/// </summary>
public class SupportsComponent : ElementComponentBase
{
    private readonly List<string> _supports = [];

    public SupportsComponent(ElementId owningElement, List<string> supports)
        : base(owningElement)
    {
        _supports = [.. supports];
    }

    /// <summary>
    /// Gets the collection of supports associated with the element.
    /// </summary>
    public IReadOnlyCollection<string> Supports => _supports.AsReadOnly();

    /// <summary>
    /// Replaces the current collection of supports with a new set.
    /// </summary>
    public void ReplaceSupports(IEnumerable<string> supports)
    {
        ArgumentNullException.ThrowIfNull(supports);

        _supports.Clear();
        _supports.AddRange(supports);
    }

    public void AddSupport(string support)
    {
        ArgumentNullException.ThrowIfNull(support);
        if (!_supports.Contains(support))
        {
            _supports.Add(support);
        }
    }

    public void RemoveSupport(string support)
    {
        ArgumentNullException.ThrowIfNull(support);
        _supports.RemoveAll(s => s == support);
    }
}
