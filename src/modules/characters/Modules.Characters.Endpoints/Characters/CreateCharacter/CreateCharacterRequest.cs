namespace Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;

public record CreateCharacterRequest(Guid CharacterCreationOptionId, string Name, string? PortraitUrl = null);
