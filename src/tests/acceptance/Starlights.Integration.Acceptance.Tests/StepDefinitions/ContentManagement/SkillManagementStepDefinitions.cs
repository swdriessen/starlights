using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.GetSkills;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class SkillManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ElementsDriverContext _driverContext;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageSkillsDriver _skillsDriver;

    public SkillManagementStepDefinitions(IIntegrationHost host, ElementsDriverContext driverContext, ScenarioContext scenarioContext)
    {
        _host = host;
        _driverContext = driverContext;
        _scenarioContext = scenarioContext;

        _skillsDriver = _host.GetDriver<ManageSkillsDriver>();
    }

    [When("the content creator creates a skill with the following properties")]
    public async Task WhenTheContentCreatorCreatesASkillWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<SkillTableRow>(_scenarioContext);

        var properties = new ManageSkillsDriver.CreateProperties
        {
            Name = row.Name,
            AbilityName = row.Ability
        };

        await _skillsDriver.CreateSkillAsync(properties);
    }

    [Then("the skill should have at least the following properties")]
    public async Task ThenTheSkillShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var current = _driverContext.CurrentElement;
        var skill = await _skillsDriver.GetSkillByIdAsync(current.Id);

        var expected = dataTable.CreateInstance<SkillTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SkillTableRow, SkillListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, skill, assertions);
    }

    [Given(@"a skill exists with the name ""(.*)"" and ability ""(.*)""")]
    public async Task GivenASkillExistsWithTheNameAndAbilityAsync(string name, string ability)
    {
        // TODO: get by name, only create if not existing

        var properties = new ManageSkillsDriver.CreateProperties
        {
            Name = name,
            AbilityName = ability
        };

        var id = await _skillsDriver.CreateSkillAsync(properties);

        _host.Set(id, $"skill-id:{name}");
    }

    [When(@"the content creator updates the skill ""(.*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheSkillWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _host.Get<Guid>($"skill-id:{name}");

        var updates = dataTable.Rows.Count == 0
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : dataTable.Rows[0]
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString(),
                    StringComparer.OrdinalIgnoreCase);

        var properties = ManageSkillsDriver.UpdateProperties.FromDictionary(updates);

        await _skillsDriver.UpdateSkillAsync(id, properties);
    }

    [Then(@"the skill ""(.*)"" should have at least the following properties")]
    public async Task ThenTheSkillShouldHaveAtLeastTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _host.Get<Guid>($"skill-id:{name}");
        var skill = await _skillsDriver.GetSkillByIdAsync(id);

        var expected = dataTable.CreateInstance<SkillTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SkillTableRow, SkillListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, skill, assertions);
    }

    #region Table Bindings

    private sealed class SkillTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    #endregion
}
