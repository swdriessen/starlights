using System.Net;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;

namespace Starlights.Integration.Tests.Core;

internal static class HttpClientCharactersModuleClassesExtensions
{
    public static Task<GetCharacterClassesResponse> GetCharacterClassesAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/classes";
        return client.GetAndReadAsync<GetCharacterClassesResponse>(url, HttpStatusCode.OK, ct);
    }
}
