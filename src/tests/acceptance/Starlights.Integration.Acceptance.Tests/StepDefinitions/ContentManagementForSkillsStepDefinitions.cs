using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public sealed class ContentManagementForSkillsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageSkillsDriver _skillsDriver;

    public ContentManagementForSkillsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _skillsDriver = _host.GetDriver<ManageSkillsDriver>();
    }

    [When(@"the content creator creates a skill with the following properties")]
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

    [Then(@"the skill should have at least the following properties")]
    public async Task ThenTheSkillShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var id = _host.Get<Guid>("last-created-skill-id");
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

    private sealed class SkillTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
