using System.Net;
using Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.GetAbilities;
using Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// Reusable HTTP client extensions for character-related endpoints.
/// </summary>
internal static class HttpClientCharactersModuleExtensions
{
    public static Task<GetCharacterCreationOptionsResponse> GetCharacterCreationOptionsAsync(this HttpClient client, CancellationToken ct = default)
        => client.GetAndReadAsync<GetCharacterCreationOptionsResponse>("/api/characters/creation-options", HttpStatusCode.OK, ct);

    public static Task<GetCharacterPortraitOptionsResponse> GetCharacterPortraitOptionsAsync(this HttpClient client, CancellationToken ct = default)
        => client.GetAndReadAsync<GetCharacterPortraitOptionsResponse>("/api/characters/portrait-options", HttpStatusCode.OK, ct);

    public static Task<CreateCharacterResponse> CreateCharacterAsync(this HttpClient client, Guid optionId, string name, string portraitUrl, CancellationToken ct = default)
        => client.PostJsonAndReadAsync<CreateCharacterResponse>("/api/characters/create", new CreateCharacterRequest(optionId, name, portraitUrl), HttpStatusCode.Created, ct);

    public static Task<GetAbilityScoresResponse> GetAbilityScoresAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
        => client.GetAndReadAsync<GetAbilityScoresResponse>($"/api/characters/{characterId}/abilities", HttpStatusCode.OK, ct);

    public static Task<HttpResponseMessage> SetAbilityBaseScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
        => client.PostJsonExpectAsync($"/api/characters/{characterId}/abilities/{abilityScoreId}/base", new { value }, HttpStatusCode.OK, ct);

    public static Task<HttpResponseMessage> SetAbilityAdditionalScoreAsync(this HttpClient client, Guid characterId, Guid abilityScoreId, int value, CancellationToken ct = default)
        => client.PostJsonExpectAsync($"/api/characters/{characterId}/abilities/{abilityScoreId}/additional", new { value }, HttpStatusCode.OK, ct);

    public static async Task WaitForAbilityScoresAsync(this HttpClient client, Guid characterId, int minCount, TimeSpan timeout, CancellationToken ct = default)
    {
        var start = DateTimeOffset.UtcNow;
        while (true)
        {
            var data = await client.GetAbilityScoresAsync(characterId, ct);
            if (data.AbilityScores.Count >= minCount)
            {
                return;
            }

            if (DateTimeOffset.UtcNow - start > timeout)
            {
                return;
            }

            await Task.Delay(100, ct);
        }
    }
}
