using System.Text.Json.Serialization;

namespace Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.Create;

public sealed record CreateProficiencyRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("proficiencyType")] string ProficiencyType,
    [property: JsonPropertyName("description")] string? Description);
