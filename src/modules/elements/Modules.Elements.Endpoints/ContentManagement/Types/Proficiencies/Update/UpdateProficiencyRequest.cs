using System.Text.Json.Serialization;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.Update;

public sealed record UpdateProficiencyRequest(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("proficiencyType")] string ProficiencyType,
    [property: JsonPropertyName("description")] string? Description);
