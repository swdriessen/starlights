using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageLanguagesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageLanguagesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Get all languages via the API <code>/api/elements/languages</code>
    /// </summary>
    public async Task<List<LanguageDataModel>> GetAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/languages", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetLanguagesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull("expected Items to be not null");

        return [.. payload.Items];
    }

    /// <summary>
    /// Create a new language via the API <code>/api/elements/languages/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateLanguageRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/languages/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateLanguageResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!.Id;
    }

    /// <summary>
    /// Get a language by ID via the API <code>/api/elements/languages/{id}</code>
    /// </summary>
    public async Task<LanguageDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/languages/{id}", _integration.CancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadFromJsonAsync<LanguageDataModel>(_integration.CancellationToken);
            responseContent.Should().NotBeNull();
            return responseContent;
        }

        return null;
    }

    /// <summary>
    /// Update an existing language via the API <code>/api/elements/languages/{id}</code>
    /// </summary>
    public async Task<bool> PutAsync(UpdateLanguageRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/languages/{request.Id}", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateLanguageResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return true;
    }
}
