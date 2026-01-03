using System.Text.Json.Serialization;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Attributes.Proficiencies.GetProficiencies;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.GetProficiencies;

public sealed record GetProficienciesResponse(
    [property: JsonPropertyName("items")] IReadOnlyCollection<ProficiencyListItem> Items);
