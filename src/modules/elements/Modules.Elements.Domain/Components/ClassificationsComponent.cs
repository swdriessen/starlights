namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that manages classifications such as labels and tags for an element.
/// </summary>
public class ClassificationsComponent : ElementComponentBase
{
    private readonly List<string> _labels = [];
    private readonly List<string> _tags = [];

    public ClassificationsComponent(ElementId owningElement)
        : base(owningElement)
    {
    }

    /// <summary>
    /// Gets a read-only collection of labels that can be used as selectors for selection rules.
    /// </summary>
    public IReadOnlyCollection<string> Labels => _labels.AsReadOnly();

    /// <summary>
    /// Gets a read-only collection of tags associated with the element that can be used for categorization or filtering.
    /// </summary>
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Replaces the current collection of labels with a new set.
    /// </summary>
    public void ReplaceLabels(IEnumerable<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        var replacements = labels.Select(l => l.Trim()).Distinct().ToList();

        _labels.Clear();

        if (replacements.Count > 0)
        {
            _labels.AddRange(replacements);
        }
    }

    public void AddLabel(string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        if (!_labels.Contains(label))
        {
            _labels.Add(label);
        }
    }

    public void RemoveLabel(string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        _labels.RemoveAll(s => s == label);
    }

    public void ReplaceTags(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);
        _tags.Clear();
        _tags.AddRange(tags);
    }

    public void AddTag(string tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        _tags.RemoveAll(s => s == tag);
    }
}