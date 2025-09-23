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
/// Reusable HTTP client extensions for character-related endpoints.
/// </summary>
internal static class HttpClientCharactersModuleExtensions
{
    public static Task<GetCharacterCreationOptionsResponse> GetCharacterCreationOptionsAsync(this HttpClient client, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetCharacterCreationOptionsResponse>("/api/characters/creation-options", HttpStatusCode.OK, ct);
    }

    public static Task<GetCharacterPortraitOptionsResponse> GetCharacterPortraitOptionsAsync(this HttpClient client, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetCharacterPortraitOptionsResponse>("/api/characters/portrait-options", HttpStatusCode.OK, ct);
    }

    public static Task<CreateCharacterResponse> CreateCharacterAsync(this HttpClient client, Guid optionId, string name, string portraitUrl, CancellationToken ct = default)
    {
        return client.PostJsonAndReadAsync<CreateCharacterResponse>("/api/characters", new CreateCharacterRequest(optionId, name, portraitUrl), HttpStatusCode.Created, ct);
    }

    public static Task<GetAbilityScoresResponse> GetAbilityScoresAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetAbilityScoresResponse>($"/api/characters/{characterId}/ability-scores", HttpStatusCode.OK, ct);
    }

    public static Task<HttpResponseMessage> SetAbilityBaseScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
    {
        return client.PostJsonExpectAsync($"/api/characters/{characterId}/ability-scores/{abilityScoreId}/base", new { value }, HttpStatusCode.OK, ct);
    }

    public static Task<HttpResponseMessage> SetAbilityAdditionalScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
    {
        return client.PostJsonExpectAsync($"/api/characters/{characterId}/ability-scores/{abilityScoreId}/additional", new { value }, HttpStatusCode.OK, ct);
    }

    public static Task<GetSkillsResponse> GetSkillsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetSkillsResponse>($"/api/characters/{characterId}/skills", HttpStatusCode.OK, ct);
    }

    public static Task<GetCharactersResponse> GetCharactersAsync(this HttpClient client, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetCharactersResponse>("/api/characters", HttpStatusCode.OK, ct);
    }

    public static Task<GetCharacterDetailsResponse> GetCharacterDetailsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        return client.GetAndReadAsync<GetCharacterDetailsResponse>($"/api/characters/{characterId}", HttpStatusCode.OK, ct);
    }

    public static async Task<HttpResponseMessage> DeleteCharacterAsync(this HttpClient client, Guid characterId, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var response = await client.DeleteAsync($"/api/characters/{characterId}", ct);
        await response.ShouldHaveStatusAsync(expected);
        return response;
    }
}
