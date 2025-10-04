using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Acceptance.Tests.StepDefinitions;

public static class HookOrder
{
    public const int INITIALIZE_ELEMENTS_DATA = 11_000;
    public const int CONFIGURE_BUILDER_WITH_OTEL = 1_500;
    public const int CONFIGURE_BUILDER_BUILD = 1_999;

}

[Binding]

public class IntegrationHostBuilderHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IntegrationHostBuilder _builder;

    public IntegrationHostBuilderHooks(TestContext testContext, ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;

        _builder = IntegrationHost.CreateBuilder()
            .WithTestContext(testContext);
    }

    [BeforeScenario(Order = 1500)]
    [Scope(Tag = "otel")]
    public void WithConsoleActivityProcessor()
    {
        _builder.WithConsoleActivityProcessor();
    }

    [BeforeScenario(Order = 1999)]
    public void BuildHost()
    {
        var host = _builder.Build();
        _scenarioContext.ScenarioContainer.RegisterInstanceAs<IIntegrationHost>(host);
    }

    [BeforeScenario(Order = 11000)] // default is 10000
    public async Task InitializeElements()
    {
        var host = _scenarioContext.ScenarioContainer.Resolve<IIntegrationHost>();
        await host.InitializeElements();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var host = _scenarioContext.ScenarioContainer.Resolve<IIntegrationHost>();
        host.Properties.Clear();
    }
}


[Binding]
public class AbilityScoresStepDefinitions
{
    private readonly IIntegrationHost _host = default!;
    private readonly CharacterCreationDriver _characterCreationDriver = default!;
    private readonly AbilityScoreDriver _abilityScoreDriver = default!;

    public AbilityScoresStepDefinitions(IIntegrationHost host)
    {
        _host = host;
        _characterCreationDriver = _host.GetDriver<CharacterCreationDriver>();
        _abilityScoreDriver = _host.GetDriver<AbilityScoreDriver>();
    }


    [Given("a new character")]
    public async Task GivenANewCharacter()
    {
        await _characterCreationDriver.CreateCharacterAsync();
    }

    [Given("the character's {word} score is {int}")]
    public async Task GivenTheCharactersIntelligenceScoreIs(string abilityName, int value)
    {
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);

        if (score.CalculatedScore == value)
        {
            return;
        }

        await _abilityScoreDriver.UpdateAbilityScoreBase(score.AbilityScoreId, value);
    }

    [When("the character's {word} score is changed to {int}")]
    public async Task WhenTheCharactersIntelligenceScoreIsChangedTo(string abilityName, int value)
    {
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);
        await _abilityScoreDriver.UpdateAbilityScoreBase(score.AbilityScoreId, value);
    }

    [Then("the character's {word} modifier should be {int}")]
    public async Task ThenTheCharactersIntelligenceModifierShouldBe(string abilityName, int value)
    {
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);
        score.CalculatedModifier.Should().Be(value, $"The {abilityName} modifier should match the expected value.");
    }

    [Then("the character's {word} score should be {int}")]
    public async Task ThenTheCharactersCharismaScoreShouldBe(string abilityName, int value)
    {
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);
        score.CalculatedScore.Should().Be(value, $"The {abilityName} score should match the expected value.");
    }
}
