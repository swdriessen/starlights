using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class ElementLabelManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ElementsDriverContext _driverContext;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ElementLabelManagementStepDefinitions(IIntegrationHost host, ElementsDriverContext driverContext, ScenarioContext scenarioContext)
    {
        _host = host;
        _driverContext = driverContext;
        _scenarioContext = scenarioContext;

        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"the content creator adds the ""([^""]*)"" label to the ""([^""]*)"" element")]
    public async Task WhenTheContentCreatorAddsTheLabelToTheElementAsync(string label, string elementName)
    {
        var element = await _elementsDriver.GetElementByName(elementName);
        await _elementsDriver.AddLabels(element.Id, [label]);
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

    [Then(@"the ""([^""]*)"" .+ should contain a ""([^""]*)"" label")]
    public async Task ThenTheElementShouldContainALabelAsync(string elementName, string label)
    {
        var elementId = _driverContext.GetElement(elementName).Id;
        var labels = await _elementsDriver.GetLabels(elementId);
        labels.Should().Contain(label);
    }

    [Then(@"the ""([^""]*)"" .+ should contain the following labels:")]
    public async Task ThenTheElementShouldContainTheFollowingLabelsAsync(string elementName, DataTable dataTable)
    {
        var expected = dataTable.CreateSet<LabelTableRow>(_scenarioContext)
            .Select(r => r.Label)
            .ToList();

        expected.Should().NotBeEmpty("at least one expected label must be provided");

        var elementId = _driverContext.GetElement(elementName).Id;
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

    #region Table Bindings

    private sealed record LabelTableRow : ITableRow
    {
        public required string Label { get; init; }
    }

    #endregion
}
