using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for character leveling related endpoints.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Updates the level of a specific class for a character.
    /// </summary>
    public static async Task<HttpResponseMessage> UpdateClassLevelAsync(this HttpClient client, Guid characterId, Guid classId, int newLevel, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/classes/{classId}/level";
        var payload = new { newLevel };
        var response = await client.PostAsJsonAsync(url, payload, ct);
        await response.ShouldHaveStatusAsync(expected);
        return response;
    }
}
