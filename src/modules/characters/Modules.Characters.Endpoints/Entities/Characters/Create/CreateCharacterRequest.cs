namespace Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;

public record CreateCharacterRequest(Guid CharacterCreationOptionId, string Name, string? PortraitUrl = null);
