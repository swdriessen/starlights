using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Languages;
using Starlights.Modules.Elements.Endpoints.Content.Languages.Create;
using Starlights.Modules.Elements.Endpoints.Content.Languages.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageLanguagesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageLanguagesEndpointDriver _api;

    public ManageLanguagesDriver(IIntegrationHost integration, ManageLanguagesEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateLanguage(string name, string kind, string? origin, string? description)
    {
        var request = new CreateLanguageRequest
        {
            Name = name,
            Kind = kind,
            Origin = origin,
            Description = description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _integration.Set(id, "last-created-language-id");

        return id;
    }

    public async Task<LanguageDataModel?> GetLanguage(Guid id)
    {
        return await _api.GetAsync(id);
    }

    public async Task<LanguageDataModel> GetLastCreatedLanguage()
    {
        var id = _integration.Get<Guid>("last-created-language-id");
        var language = await _api.GetAsync(id);
        language.Should().NotBeNull();
        return language!;
    }

    public Task<bool> UpdateLanguage(LanguageDataModel updatedModel)
    {
        var request = new UpdateLanguageRequest
        {
            Id = updatedModel.Id,
            Name = updatedModel.Name,
            Kind = updatedModel.Kind,
            Origin = updatedModel.Origin,
            Description = updatedModel.Description
        };

        return _api.PutAsync(request);
    }

    public async Task<List<LanguageDataModel>> GetLanguages()
    {
        return await _api.GetAsync();
    }
}
