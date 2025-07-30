namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents prerequisites for an element.
/// </summary>
public sealed class PrerequisitesComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrerequisitesComponent"/> class.
    /// </summary>
    public PrerequisitesComponent(ElementId owningElement, string prerequisites)
        : base(owningElement)
    {
        UpdatePrerequisites(prerequisites);
    }

    /// <summary>
    /// Gets the prerequisites string.
    /// </summary>
    public string Prerequisites { get; private set; } = string.Empty;

    /// <summary>
    /// Updates the prerequisites string.
    /// </summary>
    public void UpdatePrerequisites(string prerequisites)
    {
        if (string.IsNullOrWhiteSpace(prerequisites))
        {
            throw new ArgumentException("Prerequisites cannot be null or whitespace.", nameof(prerequisites));
        }
        Prerequisites = prerequisites.Trim();
    }
}
