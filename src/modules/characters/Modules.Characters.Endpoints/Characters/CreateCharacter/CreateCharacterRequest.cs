namespace Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;

/// <summary>Starts a named Ruleset-based build, or explicitly requests retained legacy creation.</summary>
public sealed record CreateCharacterRequest
{
    /// <summary>Creates a request using the current API contract.</summary>
    public CreateCharacterRequest() { }

    /// <summary>Preserves the retained legacy creation contract for existing callers.</summary>
    public CreateCharacterRequest(Guid CharacterCreationOptionId, string Name, string? PortraitUrl = null)
    {
        this.CharacterCreationOptionId = CharacterCreationOptionId;
        this.Name = Name;
        this.PortraitUrl = PortraitUrl;
    }

    /// <summary>Gets an optional name, limited to 100 characters.</summary>
    public string? Name { get; init; }
    /// <summary>Gets the selected registered Ruleset identifier.</summary>
    public string? RulesetId { get; init; }
    /// <summary>Gets an optional exact pack identity; omitted selection uses the sole creation option.</summary>
    public string? PackId { get; init; }
    /// <summary>Gets the exact pack version when explicitly selecting content.</summary>
    public string? PackVersion { get; init; }
    /// <summary>Gets the content-defined Character Element when explicitly selecting content.</summary>
    public string? CharacterElementId { get; init; }
    /// <summary>Gets a retained legacy catalog identifier, mutually exclusive with Ruleset-based creation.</summary>
    public Guid? CharacterCreationOptionId { get; init; }
    /// <summary>Gets a retained legacy portrait URL; not supported for Ruleset-based creation.</summary>
    public string? PortraitUrl { get; init; }
}
