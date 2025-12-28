using System.Text.Json.Serialization;

namespace Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.GetProficiencies;

public sealed record GetProficienciesResponse(
    [property: JsonPropertyName("items")] IReadOnlyCollection<ProficiencyListItem> Items);
