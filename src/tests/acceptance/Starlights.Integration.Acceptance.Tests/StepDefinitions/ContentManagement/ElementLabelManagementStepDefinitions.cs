using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class ElementLabelManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;
    private readonly ElementsScenarioContext _elementsContext;

    public ElementLabelManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
        _elementsContext = _host.Get<ElementsScenarioContext>();
    }

    [When(@"the content creator adds the ""([^""]*)"" label to the ""([^""]*)"" element")]
    public async Task WhenTheContentCreatorAddsTheLabelToTheElementAsync(string label, string elementName)
    {
        var element = await _elementsDriver.GetElementByName(elementName);
        await _elementsDriver.AddLabels(element.Id, new[] { label });
    }

    [Given(@"the content creator adds the following labels to the ""([^""]*)"" element:")]
    [When(@"the content creator adds the following labels to the ""([^""]*)"" element:")]
    public async Task WhenTheContentCreatorAddsTheFollowingLabelsToTheElementAsync(string elementName, DataTable dataTable)
    {
        var rows = dataTable.CreateSet<LabelTableRow>(_scenarioContext);
        var labels = rows.Select(r => r.Label).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        labels.Should().NotBeEmpty("at least one label must be provided");

        var element = await _elementsDriver.GetElementByName(elementName);
        await _elementsDriver.AddLabels(element.Id, labels);
    }

    [Then(@"the element ""([^""]*)"" should contain a ""([^""]*)"" label")]
    public async Task ThenTheElementShouldContainALabelAsync(string elementName, string label)
    {
        var elementId = ResolveElementId(elementName);
        var labels = await _elementsDriver.GetLabels(elementId);
        labels.Should().Contain(label);
    }

    [Then(@"the element ""([^""]*)"" should contain the following labels:")]
    public async Task ThenTheElementShouldContainTheFollowingLabelsAsync(string elementName, DataTable dataTable)
    {
        var expected = dataTable.CreateSet<LabelTableRow>(_scenarioContext)
            .Select(r => r.Label)
            .ToList();

        expected.Should().NotBeEmpty("at least one expected label must be provided");


        var elementId = ResolveElementId(elementName);
        var labels = await _elementsDriver.GetLabels(elementId);

        labels.Should().Contain(expected);
    }

    [When(@"the content creator updates the ""([^""]*)"" element with the following labels:")]
    public async Task WhenTheContentCreatorUpdatesTheElementWithTheFollowingLabelsAsync(string fireball, DataTable dataTable)
    {
        var rows = dataTable.CreateSet<LabelTableRow>(_scenarioContext);
        var labels = rows.Select(r => r.Label).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        labels.Should().NotBeEmpty("at least one label must be provided");

        var element = await _elementsDriver.GetElementByName(fireball);
        await _elementsDriver.ReplaceLabels(element.Id, labels);
    }

    private Guid ResolveElementId(string elementName)
    {
        if (_elementsContext.CreatedMap.TryGetValue(elementName, out var id))
        {
            return id;
        }

        throw new KeyNotFoundException($"No element found with name '{elementName}'. Ensure the scenario creates it using the API-backed steps.");
    }

    private sealed record LabelTableRow : ITableRow
    {
        public required string Label { get; init; }
    }
}
