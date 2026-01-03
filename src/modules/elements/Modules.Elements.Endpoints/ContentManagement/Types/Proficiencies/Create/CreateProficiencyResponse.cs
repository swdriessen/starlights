using System.Text.Json.Serialization;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.Create;

public sealed record CreateProficiencyResponse(
    [property: JsonPropertyName("id")] Guid Id);
