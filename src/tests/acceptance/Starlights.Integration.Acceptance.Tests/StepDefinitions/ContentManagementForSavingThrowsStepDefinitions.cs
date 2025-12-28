using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public sealed class ContentManagementForSavingThrowsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageSavingThrowsDriver _savingThrowsDriver;

    public ContentManagementForSavingThrowsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _savingThrowsDriver = _host.GetDriver<ManageSavingThrowsDriver>();
    }

    [When(@"a content creator creates a saving throw with the following properties")]
    public async Task WhenAContentCreatorCreatesASavingThrowWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var properties = new ManageSavingThrowsDriver.CreateProperties
        {
            Name = row.Name,
            AbilityName = row.Ability
        };

        await _savingThrowsDriver.CreateSavingThrowAsync(properties);
    }

    [Then(@"the saving throw should have at least the following properties")]
    public async Task ThenTheSavingThrowShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var id = _host.Get<Guid>("last-created-saving-throw-id");
        var savingThrow = await _savingThrowsDriver.GetSavingThrowByIdAsync(id);

        var expected = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SavingThrowTableRow, SavingThrowListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, savingThrow, assertions);
    }

    [Given(@"a saving throw exists with the name ""(.*)"" and ability ""(.*)""")]
    public async Task GivenASavingThrowExistsWithTheNameAndAbilityAsync(string name, string ability)
    {
        var id = await _savingThrowsDriver.CreateSavingThrowAsync(
            new ManageSavingThrowsDriver.CreateProperties
            {
                Name = name,
                AbilityName = ability
            },
            storeAsLastCreated: false);

        _host.Set(id, $"saving-throw-id:{name}");
    }

    [When(@"the content creator updates the saving throw ""(.*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheSavingThrowWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _host.Get<Guid>($"saving-throw-id:{name}");

        var updates = dataTable.Rows.Count == 0
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : dataTable.Rows[0]
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString(),
                    StringComparer.OrdinalIgnoreCase);

        var properties = ManageSavingThrowsDriver.UpdateProperties.FromDictionary(updates);

        await _savingThrowsDriver.UpdateSavingThrowAsync(id, properties);
    }

    [Then(@"the saving throw ""(.*)"" should have at least the following properties")]
    public async Task ThenTheSavingThrowShouldHaveAtLeastTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _host.Get<Guid>($"saving-throw-id:{name}");
        var savingThrow = await _savingThrowsDriver.GetSavingThrowByIdAsync(id);

        var expected = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SavingThrowTableRow, SavingThrowListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, savingThrow, assertions);
    }

    private sealed class SavingThrowTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
