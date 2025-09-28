using System.Net;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

namespace Starlights.Integration.Tests.Core;

internal static class HttpClientCharactersModuleRegistrationsExtensions
{
    public static Task<GetRegistrationsResponse> GetRegistrationsAsync(this HttpClient client, Guid characterId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/registrations";
        return client.GetAndReadAsync<GetRegistrationsResponse>(url, HttpStatusCode.OK, ct);
    }
}
