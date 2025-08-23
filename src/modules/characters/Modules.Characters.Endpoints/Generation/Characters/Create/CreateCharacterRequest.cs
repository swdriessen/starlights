namespace Starlights.Modules.Characters.Endpoints.Generation.Characters.Create;

public record CreateCharacterRequest(Guid CharacterCreationOptionId, string Name, string? PortraitUrl = null);
