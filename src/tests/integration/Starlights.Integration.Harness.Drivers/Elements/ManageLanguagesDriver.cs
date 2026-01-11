using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageLanguagesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly ManageLanguagesEndpointDriver _api;

    public ManageLanguagesDriver(IIntegrationHost integration, ElementsDriverContext driverContext, ManageLanguagesEndpointDriver endpointDriver)
    {
        _integration = integration;
        _driverContext = driverContext;
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

        _driverContext.WithCreatedElement(id, request.Name);

        return id;
    }

    public async Task<LanguageDataModel?> GetLanguage(Guid id)
    {
        return await _api.GetAsync(id);
    }

    public async Task<LanguageDataModel> GetLastCreatedLanguage()
    {
        var language = await _api.GetAsync(_driverContext.CurrentElement.Id);
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
