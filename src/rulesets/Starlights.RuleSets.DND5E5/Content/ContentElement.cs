namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// A validated Element composed from shared identity, Rules and reusable typed properties.
/// </summary>
public sealed record ContentElement(string Id, string Name, string Type, string? Description,
    IReadOnlyList<ContentRule> Rules, IReadOnlyList<IElementProperty> Properties, IReadOnlyList<SheetEntry> Entries,
    IReadOnlyDictionary<string, SheetValue> SheetValues)
{

    /// <summary>
    /// Reads a supported composition contract without reinterpreting raw JSON.
    /// </summary>
    public T? Property<T>() where T : class, IElementProperty
    {
        return Properties.OfType<T>().SingleOrDefault();
    }
}
