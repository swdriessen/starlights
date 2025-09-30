using System.Net;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Characters.Skills.GetSkills;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for core Character endpoints.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Creates a new character using the provided creation option, name and portrait.
    /// </summary>
    public static Task<CreateCharacterResponse> CreateCharacterAsync(this HttpClient client, Guid optionId, string name, string portraitUrl, CancellationToken ct = default)
    {
        var url = "/api/characters";
        var payload = new CreateCharacterRequest(optionId, name, portraitUrl);
        return client.PostJsonAndReadAsync<CreateCharacterResponse>(url, payload, HttpStatusCode.Created, ct);
    }

    /// <summary>
    /// Deletes an existing character.
    /// </summary>
    public static async Task<HttpResponseMessage> DeleteCharacterAsync(this HttpClient client, Guid characterId, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}";
        var response = await client.DeleteAsync(url, ct);
        await response.ShouldHaveStatusAsync(expected);
        return response;
    }

    /// <summary>
    /// Retrieves all ability scores for the specified character.
    /// </summary>
    public static Task<GetAbilityScoresResponse> GetAbilityScoresAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/ability-scores";
        return client.GetAndReadAsync<GetAbilityScoresResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves character creation options.
    /// </summary>
    public static Task<GetCharacterCreationOptionsResponse> GetCharacterCreationOptionsAsync(this HttpClient client, CancellationToken ct = default)
    {
        var url = "/api/characters/creation-options";
        return client.GetAndReadAsync<GetCharacterCreationOptionsResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves detailed character information.
    /// </summary>
    public static Task<GetCharacterDetailsResponse> GetCharacterDetailsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}";
        return client.GetAndReadAsync<GetCharacterDetailsResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves portrait options for character creation.
    /// </summary>
    public static Task<GetCharacterPortraitOptionsResponse> GetCharacterPortraitOptionsAsync(this HttpClient client, CancellationToken ct = default)
    {
        var url = "/api/characters/portrait-options";
        return client.GetAndReadAsync<GetCharacterPortraitOptionsResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves the list of all characters.
    /// </summary>
    public static Task<GetCharactersResponse> GetCharactersAsync(this HttpClient client, CancellationToken ct = default)
    {
        var url = "/api/characters";
        return client.GetAndReadAsync<GetCharactersResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves the skills for the specified character.
    /// </summary>
    public static Task<GetSkillsResponse> GetSkillsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/skills";
        return client.GetAndReadAsync<GetSkillsResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Sets the additional score for a specific ability of a character.
    /// </summary>
    public static Task<HttpResponseMessage> SetAbilityAdditionalScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/ability-scores/{abilityScoreId}/additional";
        var payload = new { value };
        return client.PostJsonExpectAsync(url, payload, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Sets the base score for a specific ability of a character.
    /// </summary>
    public static Task<HttpResponseMessage> SetAbilityBaseScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/ability-scores/{abilityScoreId}/base";
        var payload = new { value };
        return client.PostJsonExpectAsync(url, payload, HttpStatusCode.OK, ct);
    }
}
