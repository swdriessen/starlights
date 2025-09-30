using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for character builder selection registrations.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Registers a selection rule for a character builder flow.
    /// </summary>
    public static async Task<Guid> RegisterSelectionRuleAsync(this HttpClient client, Guid characterId, Guid parentRegistrationId, Guid selectionRuleId, Guid elementId, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/builder/selection-rules/{selectionRuleId}/register";
        var payload = new { characterId, parentRegistration = parentRegistrationId, elementId, selectionRuleId };
        var response = await client.PostAsJsonAsync(url, payload, ct);
        await response.ShouldHaveStatusAsync(expected);

        var dto = await response.Content.ReadFromJsonAsync<RegisterSelectionRuleResponse>(ct);
        dto.Should().NotBeNull();
        dto!.RegistrationId.Should().NotBe(Guid.Empty);
        return dto.RegistrationId;
    }
}
