using System.Net;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// HTTP client extensions for selection rule queries in the character builder.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Retrieves selection rules available for a character.
    /// </summary>
    public static Task<GetSelectionRulesResponse> GetSelectionRulesAsync(this HttpClient client, Guid characterId, string[] types, CancellationToken ct = default)
    {
        var query = types is { Length: > 0 }
            ? string.Concat(types.Select(t => $"&type={Uri.EscapeDataString(t)}"))
            : string.Empty;
        var url = $"/api/characters/{characterId}/builder/selection-rules?{query.TrimStart('&')}";
        return client.GetAndReadAsync<GetSelectionRulesResponse>(url, HttpStatusCode.OK, ct);
    }

    /// <summary>
    /// Retrieves selectable options for a selection rule.
    /// </summary>
    public static Task<GetSelectionRuleOptionsResponse> GetSelectionRuleOptionsAsync(this HttpClient client, Guid characterId, Guid selectionRuleId, CancellationToken ct = default)
    {
        var url = $"/api/characters/{characterId}/builder/selection-rules/{selectionRuleId}/options";
        return client.GetAndReadAsync<GetSelectionRuleOptionsResponse>(url, HttpStatusCode.OK, ct);
    }
}
