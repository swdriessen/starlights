namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ClassManagementStepDefinitions
{
    [When(@"the content creator creates a ""([^""]*)"" class with a ""([^""]*)"" hit point die")]
    public async Task WhenTheContentCreatorCreatesAClassWithAHitPointDieAsync(string barbarian, string p1)
    {
        throw new PendingStepException();
    }

    [Then(@"the ""([^""]*)"" class should have at least the following properties")]
    public async Task ThenTheClassShouldHaveAtLeastTheFollowingPropertiesAsync(string barbarian, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"a class exists with the name ""([^""]*)""")]
    public async Task GivenAClassExistsWithTheNameAsync(string barbarian)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator creates a level (.*) class feature for the ""([^""]*)"" class with the name ""([^""]*)""")]
    public async Task WhenTheContentCreatorCreatesALevelClassFeatureForTheClassWithTheNameAsync(int p0, string barbarian, string p2)
    {
        throw new PendingStepException();
    }

    [Then(@"the class feature should have at least the following properties")]
    public async Task ThenTheClassFeatureShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator creates a subclass for the ""([^""]*)"" class with the name ""([^""]*)""")]
    public async Task WhenTheContentCreatorCreatesASubclassForTheClassWithTheNameAsync(string barbarian, string p1)
    {
        throw new PendingStepException();
    }

    [Then(@"the subclass should have at least the following properties")]
    public async Task ThenTheSubclassShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"a class exists with the name ""([^""]*)"" with the following class features:")]
    public async Task GivenAClassExistsWithTheNameWithTheFollowingClassFeaturesAsync(string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator adds a new spellcasting rule to the ""([^""]*)"" class feature with the following details:")]
    public async Task WhenTheContentCreatorAddsANewSpellcastingRuleToTheClassFeatureWithTheFollowingDetailsAsync(string spellcasting, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ""([^""]*)"" element should have at least the following properties")]
    public async Task ThenTheElementShouldHaveAtLeastTheFollowingPropertiesAsync(string spellcasting, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"the following spells with their respective properties exists")]
    public async Task GivenTheFollowingSpellsWithTheirRespectivePropertiesExistsAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"a spellcaster class exists with the name ""([^""]*)"" and a spellcasting feature named ""([^""]*)""")]
    public async Task GivenASpellcasterClassExistsWithTheNameAndASpellcastingFeatureNamedAsync(string wizard, string spellcasting)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator updates to the spellcasting rule of the ""([^""]*)"" class feature with the following ""([^""]*)"" spell list:")]
    public async Task WhenTheContentCreatorUpdatesToTheSpellcastingRuleOfTheClassFeatureWithTheFollowingSpellListAsync(string spellcasting, string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ""([^""]*)"" class feature should have at least the following spells in the ""([^""]*)"" spell list:")]
    public async Task ThenTheClassFeatureShouldHaveAtLeastTheFollowingSpellsInTheSpellListAsync(string spellcasting, string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }
}
