namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that identifies the origin of a language for an element.
/// </summary>
public sealed class LanguageComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The element this component belongs to.</param>
    /// <param name="origin">The origin of the language.</param>
    public LanguageComponent(ElementId owningElement, string origin)
        : base(owningElement)
    {
        UpdateOrigin(origin);
    }

    /// <summary>
    /// Gets the origin of the language.
    /// </summary>
    public string Origin { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the kind of the language. Defaults to "Standard".
    /// </summary>
    public string Kind { get; private set; } = "Standard";

    /// <summary>
    /// Updates the origin of the language. Value can be empty but not null.
    /// </summary>
    /// <param name="origin">The new origin value.</param>
    public void UpdateOrigin(string origin)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));

        Origin = origin.Trim();
    }

    /// <summary>
    /// Updates the kind of the language. Value can be any non-null string.
    /// </summary>
    /// <param name="kind">The new kind value.</param>
    public void UpdateKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
        {
            throw new ArgumentException("Kind cannot be null or whitespace.", nameof(kind));
        }
        Kind = kind.Trim();
    }
}
