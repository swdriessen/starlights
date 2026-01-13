using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public class LanguageManagementStepDefinitions
{
    public const string CURRENT_LANGUAGE_TABLE_ROW = "CURRENT-LANGUAGE-ROW";

    private readonly ScenarioContext _scenarioContext;
    private readonly IIntegrationHost _host;
    private readonly ManageLanguagesDriver _driver;

    public LanguageManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _driver = _host.GetDriver<ManageLanguagesDriver>();
    }

    [When("the content creator creates a language with the following properties")]
    public async Task WhenTheContentCreatorCreatesALanguageWithTheFollowingProperties(DataTable dataTable)
    {
        var properties = dataTable.CreateInstance<CreateLanguageTableRow>(_scenarioContext);

        _scenarioContext.Set(properties, CURRENT_LANGUAGE_TABLE_ROW);

        await _driver.CreateLanguage(properties.Name, properties.Kind, properties.Origin, properties.Description);
    }

    [Then("the language appears in the language list with all provided properties")]
    public async Task ThenTheLanguageAppearsInTheLanguageListWithAllProvidedProperties()
    {
        var expectedProperties = _scenarioContext.Get<CreateLanguageTableRow>(CURRENT_LANGUAGE_TABLE_ROW);

        var language = await _driver.GetLastCreatedLanguage();

        language.Name.Should().Be(expectedProperties.Name);
        language.Kind.Should().Be(expectedProperties.Kind);

        if (expectedProperties.Origin is not null)
        {
            language.Origin.Should().Be(expectedProperties.Origin);
        }

        if (expectedProperties.Description is not null)
        {
            language.Description.Should().Be(expectedProperties.Description);
        }

        var list = await _driver.GetLanguages();
        list.Should().Contain(l => l.Id == language.Id);
    }

    [Given("a language exists that includes the following properties")]
    public async Task GivenALanguageExistsThatIncludesTheFollowingProperties(DataTable dataTable)
    {
        var createRow = new CreateLanguageTableRow
        {
            Name = "Name",
            Kind = "Standard",
            Origin = null,
            Description = string.Empty
        };

        dataTable.FillInstance(createRow);

        await _driver.CreateLanguage(createRow.Name, createRow.Kind, createRow.Origin, createRow.Description);
    }

    [When("the content creator updates the language to have the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheLanguageToHaveTheFollowingProperties(DataTable dataTable)
    {
        var existing = await _driver.GetLastCreatedLanguage();

        var updates = dataTable.CreateInstance<UpdateLanguageTableRow>(_scenarioContext);

        var updated = existing with
        {
            Name = updates.Name ?? existing.Name,
            Kind = updates.Kind ?? existing.Kind,
            Origin = updates.Origin ?? existing.Origin,
            Description = updates.Description ?? existing.Description
        };

        var ok = await _driver.UpdateLanguage(updated);
        ok.Should().BeTrue();
    }

    [Then("the language in the language list should have the following properties")]
    public async Task ThenTheLanguageInTheLanguageListShouldHaveTheFollowingProperties(DataTable dataTable)
    {
        var language = await _driver.GetLastCreatedLanguage();

        var expected = dataTable.CreateInstance<UpdateLanguageTableRow>(_scenarioContext);

        var provided = dataTable.Header
            .Select(h => h.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var assertions = new Dictionary<string, Action<UpdateLanguageTableRow, LanguageDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["kind"] = (e, a) => a.Kind.Should().Be(e.Kind),
            ["origin"] = (e, a) => a.Origin.Should().Be(e.Origin ?? string.Empty),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description)
        };

        foreach (var header in provided)
        {
            if (assertions.TryGetValue(header, out var assert))
            {
                assert(expected, language);
            }
            else
            {
                throw new NotImplementedException($"checking property '{header}' is not implemented");
            }
        }
    }


    #region Table Bindings

    private sealed record CreateLanguageTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string? Origin { get; set; }
        public string? Description { get; set; } = string.Empty;
    }

    private sealed record UpdateLanguageTableRow : IMarkdownDescriptionTableRow
    {
        public string? Name { get; set; }
        public string? Kind { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; }
    }

    #endregion
}
