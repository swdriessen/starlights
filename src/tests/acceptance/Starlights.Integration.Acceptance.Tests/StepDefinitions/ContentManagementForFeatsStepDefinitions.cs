namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;

[Binding]
public class ContentManagementForFeatsStepDefinitions
{
    public const string CURRENT_FEAT_CATEGORY_NAME = "CURRENT-FEAT-CATEGORY-NAME";

    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageFeatCategoriesDriver _driver;

    public ContentManagementForFeatsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _driver = _host.GetDriver<ManageFeatCategoriesDriver>();
    }

    private sealed class FeatCategoryTableRow
    {
        public required string Name { get; set; }
    }

    [Given("the following feat categories exist")]
    public async Task GivenTheFollowingFeatCategoriesExist(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<FeatCategoryTableRow>().ToList();
        rows.Should().NotBeEmpty("expected at least one feat category row to be provided");

        var existing = await _driver.GetFeatCategories();

        foreach (var row in rows)
        {
            var alreadyExists = existing.Any(x => string.Equals(x.Name, row.Name, StringComparison.OrdinalIgnoreCase));
            if (alreadyExists)
            {
                continue;
            }

            var id = await _driver.CreateFeatCategory(row.Name, description: null);
            id.Should().NotBeEmpty();
        }
    }

    [When("the content creator creates a feat category with the name {string}")]
    public async Task WhenTheContentCreatorCreatesAFeatCategoryWithTheName(string name)
    {
        _scenarioContext.Set(name, CURRENT_FEAT_CATEGORY_NAME);

        var id = await _driver.CreateFeatCategory(name, description: null);
        id.Should().NotBeEmpty();
    }

    [Then("the feat category should exist in feat category list with the name {string}")]
    public async Task ThenTheFeatCategoryShouldExistInFeatCategoryListWithTheName(string name)
    {
        var list = await _driver.GetFeatCategories();

        list.Should().Contain(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    [Given("a feat category exists with the name {string}")]
    public async Task GivenAFeatCategoryExistsWithTheName(string name)
    {
        var existing = await _driver.GetFeatCategories();
        if (existing.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            _scenarioContext.Set(name, CURRENT_FEAT_CATEGORY_NAME);
            return;
        }

        var id = await _driver.CreateFeatCategory(name, description: null);
        id.Should().NotBeEmpty();

        _scenarioContext.Set(name, CURRENT_FEAT_CATEGORY_NAME);
    }

    [When("the content creator updates the feat category to have the name {string}")]
    public async Task WhenTheContentCreatorUpdatesTheFeatCategoryToHaveTheName(string name)
    {
        var existing = await _driver.GetLastCreatedFeatCategory();

        var updated = existing with { Name = name };

        var ok = await _driver.UpdateFeatCategory(updated);
        ok.Should().BeTrue();

        _scenarioContext.Set(name, CURRENT_FEAT_CATEGORY_NAME);
    }

    [When("the content creator creates a feat with the following properties")]
    public void WhenTheContentCreatorCreatesAFeatWithTheFollowingProperties(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then("the feat should exist in the feat list with all provided properties")]
    public void ThenTheFeatShouldExistInTheFeatListWithAllProvidedProperties()
    {
        throw new PendingStepException();
    }

    [Given("a feat exists with the name {string} and category {string}")]
    public void GivenAFeatExistsWithTheNameAndCategory(string p0, string p1)
    {
        throw new PendingStepException();
    }
}
