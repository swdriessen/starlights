using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Feats;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public class FeatManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageFeatCategoriesDriver _featCategoriesDriver;
    private readonly ManageFeatsDriver _featsDriver;

    public FeatManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _featCategoriesDriver = _host.GetDriver<ManageFeatCategoriesDriver>();
        _featsDriver = _host.GetDriver<ManageFeatsDriver>();
    }

    [Given("the following feat categories exist")]
    public async Task GivenTheFollowingFeatCategoriesExist(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<FeatCategoryTableRow>().ToList();
        rows.Should().NotBeEmpty("expected at least one feat category row to be provided");

        await _featCategoriesDriver.CreateFeatCategoriesWhenNotExisting(rows.Select(x => x.Name));
    }

    [When("the content creator creates a feat category with the name {string}")]
    public async Task WhenTheContentCreatorCreatesAFeatCategoryWithTheName(string name)
    {
        await _featCategoriesDriver.CreateFeatCategory(name);
    }

    [Then("the feat category should exist in feat category list with the name {string}")]
    public async Task ThenTheFeatCategoryShouldExistInFeatCategoryListWithTheName(string name)
    {
        var exists = await _featCategoriesDriver.ExistsFeatCategory(name);
        exists.Should().BeTrue($"expected feat category '{name}' to exist");
    }

    [Given("a feat category exists with the name {string}")]
    public async Task GivenAFeatCategoryExistsWithTheName(string name)
    {
        await _featCategoriesDriver.CreateFeatCategoriesWhenNotExisting([name]);
    }

    [When("the content creator updates the feat category to have the name {string}")]
    public async Task WhenTheContentCreatorUpdatesTheFeatCategoryToHaveTheName(string name)
    {
        var existing = await _featCategoriesDriver.GetLastCreatedFeatCategory();
        var updated = existing with { Name = name };

        await _featCategoriesDriver.UpdateFeatCategory(updated);
    }

    [When("the content creator creates a feat with the following properties")]
    public async Task WhenTheContentCreatorCreatesAFeatWithTheFollowingProperties(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<FeatTableRow>(_scenarioContext);

        if (row.Category is null)
        {
            throw new InvalidOperationException("Feat category must be provided");
        }

        var category = await _featCategoriesDriver.GetFeatCategoryByName(row.Category);

        var properties = new ManageFeatsDriver.CreateProperties
        {
            Name = row.Name,
            CategoryId = category.Id,
            ShortDescription = row.ShortDescription,
            Description = row.Description,
            IsRepeatable = row.Repeatable,
            Prerequisite = row.Prerequisite
        };

        await _featsDriver.CreateFeat(properties);
    }

    [Then("the feat should exist in the feat list with all provided properties")]
    public async Task ThenTheFeatShouldExistInTheFeatListWithAllProvidedProperties()
    {
        var feat = await _featsDriver.GetLastCreatedFeat();

        var expected = _scenarioContext.Get<FeatTableRow>();

        feat.Name.Should().Be(expected.Name);
        feat.CategoryId.Should().NotBeEmpty("the id of the category is not in the feature file, rather the category name the content creator interacts with...");
        feat.Category.Should().Be(expected.Category);
        feat.ShortDescription.Should().Be(expected.ShortDescription ?? string.Empty);
        feat.Description.Should().Be(expected.Description ?? string.Empty);
        feat.IsRepeatable.Should().Be(expected.Repeatable);
        feat.Prerequisites.Should().Be(expected.Prerequisite ?? string.Empty);
    }

    [Given("a feat exists with the name {string} and category {string}")]
    public async Task GivenAFeatExistsWithTheNameAndCategory(string name, string category)
    {
        var feats = await _featsDriver.GetFeats();

        if (feats.Any(f => f.Name == name && f.Category == category))
        {
            return;
        }

        var categoryElement = await _featCategoriesDriver.GetFeatCategoryByName(category);

        if (categoryElement != null)
        {
            var properties = new ManageFeatsDriver.CreateProperties
            {
                Name = name,
                CategoryId = categoryElement.Id,
                ShortDescription = string.Empty,
                Description = string.Empty,
                IsRepeatable = false,
                Prerequisite = string.Empty
            };
            await _featsDriver.CreateFeat(properties);
        }
        else
        {
            throw new InvalidOperationException($"Feat category '{category}' does not exist.");
        }
    }

    [Then(@"the feat should have at least the following properties")]
    public async Task ThenThisFeatShouldHaveTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var feat = await _featsDriver.GetLastCreatedFeat();

        var expected = dataTable.CreateInstance<FeatTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<FeatTableRow, FeatDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["category"] = (e, a) => a.Category.Should().Be(e.Category),
            ["repeatable"] = (e, a) => a.IsRepeatable.Should().Be(e.Repeatable),
            ["prerequisite"] = (e, a) => a.Prerequisites.Should().Be(e.Prerequisite),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description)
        };

        dataTable.AssertProvidedProperties(expected, feat, assertions);
    }

    [Given(@"a feat exists that includes the following properties")]
    public async Task GivenAFeatExistsThatIncludesTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<FeatTableRow>(_scenarioContext);

        if (row.Category is null)
        {
            throw new InvalidOperationException("Feat category must be provided");
        }

        var category = await _featCategoriesDriver.GetorCreateFeatCategoryByName(row.Category);

        var properties = new ManageFeatsDriver.CreateProperties
        {
            Name = row.Name,
            CategoryId = category.Id,
            ShortDescription = row.ShortDescription,
            Description = row.Description,
            IsRepeatable = row.Repeatable,
            Prerequisite = row.Prerequisite,
        };

        await _featsDriver.CreateFeat(properties);
    }

    [When(@"the content creator updates the feat with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheFeatWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<UpdateFeatTableRow>(_scenarioContext);

        var existingFeat = await _featsDriver.GetLastCreatedFeat();

        var updatedFeat = existingFeat with
        {
            Name = row.Name ?? existingFeat.Name,
            ShortDescription = row.ShortDescription ?? existingFeat.ShortDescription,
            Description = row.Description ?? existingFeat.Description,
            IsRepeatable = row.Repeatable ?? existingFeat.IsRepeatable,
            Prerequisites = row.Prerequisite ?? existingFeat.Prerequisites
        };

        if (row.Category is not null && row.Category != existingFeat.Category)
        {
            var category = await _featCategoriesDriver.GetorCreateFeatCategoryByName(row.Category);
            updatedFeat = updatedFeat with { CategoryId = category.Id, Category = category.Name };
        }

        await _featsDriver.UpdateFeat(updatedFeat);
    }

    #region Table Bindings

    private sealed class FeatCategoryTableRow : ITableRow
    {
        public required string Name { get; set; }
    }

    private sealed class FeatTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public string? Prerequisite { get; set; }
        public bool Repeatable { get; set; }
    }

    private sealed class UpdateFeatTableRow : IMarkdownDescriptionTableRow
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public string? Prerequisite { get; set; }
        public bool? Repeatable { get; set; }
    }
    #endregion
}
