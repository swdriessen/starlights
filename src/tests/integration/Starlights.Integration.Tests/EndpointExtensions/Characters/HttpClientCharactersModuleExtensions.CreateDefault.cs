using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// Scenario-style helpers for composing multi-step character setup.
/// </summary>
internal static partial class HttpClientCharactersModuleExtensions
{
    /// <summary>
    /// Creates a default test character using the first available creation option and portrait.
    /// </summary>
    /// <remarks>
    /// Does NOT call InitializeElementsAsync; call that separately if needed.
    /// </remarks>
    public static async Task<Guid> CreateDefaultCharacterAsync(this HttpClient client, CancellationToken ct = default)
    {
        var options = await client.GetCharacterCreationOptionsAsync(ct);
        var portraits = await client.GetCharacterPortraitOptionsAsync(ct);

        var optionId = options.Options.First().Id; // deterministic for tests
        var portraitUrl = portraits.Portraits.First().Url;

        var created = await client.CreateCharacterAsync(optionId, $"ITest {Guid.NewGuid()}", portraitUrl, ct);
        return created.Id;
    }
}
