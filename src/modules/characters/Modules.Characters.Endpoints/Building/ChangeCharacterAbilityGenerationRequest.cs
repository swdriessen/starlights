namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Changes generation method or explicitly rerolls; replacement of assignments requires confirmation.</summary>
public sealed record ChangeCharacterAbilityGenerationRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, string Method, bool Reroll = false, bool ConfirmReplacement = false) : ICharacterEditRequest;
