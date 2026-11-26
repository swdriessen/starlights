namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A rebuildable sheet calculated for exactly the containing snapshot's revision.</summary>
public sealed record CharacterSheetResponse(IReadOnlyList<CharacterStatisticResponse> Statistics,
    IReadOnlyList<CharacterAttackResponse> Attacks, IReadOnlyList<CharacterSheetEntryResponse> Entries, int Gold);
