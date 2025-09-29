using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Starlights.Integration.Tests.Core;

internal static class HttpClientCharactersModuleLevelUpExtensions
{
    public static async Task<HttpResponseMessage> UpdateClassLevelAsync(this HttpClient client, Guid characterId, Guid classId, int newLevel, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/classes/{classId}/level";
        var payload = new { newLevel };
        var response = await client.PostAsJsonAsync(url, payload, ct);
        await response.ShouldHaveStatusAsync(expected);
        return response;
    }
}
