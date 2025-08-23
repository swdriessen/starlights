namespace Starlights.Modules.Characters.Endpoints.Characters.Create;

public record CreateCharacterRequest(Guid CharacterCreationOptionId, string Name, string? PortraitUrl = null);
