namespace Starlights.RuleSets.DND5E5;

/// <summary>A validated Element composed from shared identity, Rules and reusable typed properties.</summary>
public sealed record DndContentElement(string Id, string Name, string Type, string? Description,
    IReadOnlyList<DndContentRule> Rules, IReadOnlyList<IDndElementProperty> Properties, IReadOnlyList<DndSheetEntry> Entries,
    IReadOnlyDictionary<string, DndSheetValue> SheetValues)
{
    /// <summary>Reads a supported composition contract without reinterpreting raw JSON.</summary>
    public T? Property<T>() where T : class, IDndElementProperty => Properties.OfType<T>().SingleOrDefault();
}
