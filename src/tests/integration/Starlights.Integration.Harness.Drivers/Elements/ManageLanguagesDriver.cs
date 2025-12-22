using AwesomeAssertions;
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
        _integration.WriteLine($"creating language {name}");

        var request = new CreateLanguageRequest
        {
            Name = name,
            Kind = kind,
            Origin = origin,
            Description = description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _integration.Properties["last-created-language-id"] = id;

        return id;
    }

    public async Task<LanguageDataModel?> GetLanguage(Guid id)
    {
        _integration.WriteLine($"retrieving language {id}");

        var language = await _api.GetAsync(id);

        if (language is not null)
        {
            _integration.Properties["last-retrieved-language"] = language;
        }

        return language;
    }

    public async Task<LanguageDataModel> GetLastCreatedLanguage()
    {
        var id = (Guid)_integration.Properties["last-created-language-id"]!;
        var language = await GetLanguage(id);
        language.Should().NotBeNull();
        return language!;
    }

    public Task<bool> UpdateLanguage(LanguageDataModel updatedModel)
    {
        _integration.WriteLine($"updating language {updatedModel.Name} ({updatedModel.Id})");

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
        var languages = await _api.GetAsync();

        _integration.Properties["last-retrieved-languages"] = languages;

        _integration.WriteLine($"retrieved {languages.Count} languages.");

        foreach (var language in languages)
        {
            _integration.WriteLine($"- {language.Name} ({language.Kind})");
        }

        return languages;
    }
}
