namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>An authoritative revision-qualified build snapshot; acceptedRevision identifies a submitted operation's original receipt.</summary>
public sealed record CharacterBuildResponse(
    Guid CharacterId, string Name, long Revision, string Status, Guid? OperationId, string? Failure,
    bool BuildComplete, bool SheetReady, IReadOnlyList<CharacterSelectionResponse> Selections,
    IReadOnlyList<CharacterAbilityResponse> Abilities, IReadOnlyList<CharacterEquipmentClaimResponse> EquipmentClaims,
    IReadOnlyList<CharacterOwnedItemResponse> OwnedItems, string? ArmorId, string? WeaponId,
    CharacterSheetResponse? Sheet, IReadOnlyList<int> AbilityInputOptions,
    CharacterAbilityGenerationResponse AbilityGeneration, long? AcceptedRevision = null);
