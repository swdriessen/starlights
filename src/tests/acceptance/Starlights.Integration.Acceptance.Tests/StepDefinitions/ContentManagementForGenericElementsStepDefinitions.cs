using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForGenericElementsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForGenericElementsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"the content creator creates an element with the following properties")]
    public async Task WhenTheContentCreatorCreatesAnElementWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<ElementTableRow>(_scenarioContext);

        var properties = new ManageElementsDriver.CreateProperties
        {
            Name = row.Name,
            Type = row.Type,
            Description = row.Description
        };

        await _elementsDriver.CreateElement(properties);
    }

    [Then(@"the element should have at least the following properties")]
    public async Task ThenTheElementShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var element = await _elementsDriver.GetLastCreatedElement();
        var expected = dataTable.CreateInstance<ElementTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<ElementTableRow, ElementDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["type"] = (e, a) => a.Type.Should().Be(e.Type),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, element, assertions);
    }

    private sealed class ElementTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }
}
