using System.Text.Json.Serialization;

namespace Starlights.Modules.Elements.Endpoints.Content.Proficiencies.GetProficiencies;

public sealed record GetProficienciesResponse(
    [property: JsonPropertyName("items")] IReadOnlyCollection<ProficiencyListItem> Items);
