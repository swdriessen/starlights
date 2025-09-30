using System.Net;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for retrieving character builder registrations.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Retrieves builder registrations for a character.
    /// </summary>
    public static Task<GetRegistrationsResponse> GetRegistrationsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/registrations";
        return client.GetAndReadAsync<GetRegistrationsResponse>(url, HttpStatusCode.OK, ct);
    }
}
