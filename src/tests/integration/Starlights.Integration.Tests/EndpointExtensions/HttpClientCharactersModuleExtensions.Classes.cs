using System.Net;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for character class related endpoints.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Retrieves the classes for the specified character.
    /// </summary>
    public static Task<GetCharacterClassesResponse> GetCharacterClassesAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/classes";
        return client.GetAndReadAsync<GetCharacterClassesResponse>(url, HttpStatusCode.OK, ct);
    }
}
