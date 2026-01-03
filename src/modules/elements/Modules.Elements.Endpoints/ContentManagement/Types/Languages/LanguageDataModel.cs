namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;

public sealed record LanguageDataModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Kind { get; init; } = string.Empty;
    public string Origin { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
