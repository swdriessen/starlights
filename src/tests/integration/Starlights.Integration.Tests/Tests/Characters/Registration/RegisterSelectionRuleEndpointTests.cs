using FluentAssertions;
using Starlights.Integration.Constants;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Registration;

[TestClass]
public sealed class RegisterSelectionRuleEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private RegistrationDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _driver = _integration.GetDriver<RegistrationDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSelectionRules_Returns_Rules()
    {
        // Act
        var rules = await _driver.GetSelectionRules([SelectionRuleTypes.Class, SelectionRuleTypes.Species, SelectionRuleTypes.Background]);

        // Assert
        rules.Should().NotBeEmpty();
        rules.Should().AllSatisfy(r =>
        {
            r.RegistrationId.Should().NotBe(Guid.Empty);
            r.RegistrationSelectionRuleId.Should().NotBe(Guid.Empty);
            r.Name.Should().NotBeNullOrWhiteSpace();
            r.Type.Should().NotBeNullOrWhiteSpace();
            // check type
            r.Type.Should().BeOneOf(SelectionRuleTypes.Class, SelectionRuleTypes.Species, SelectionRuleTypes.Background);
        });
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSelectionRuleOptions_Returns_Options()
    {
        // Arrange        
        var rule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class);

        // Act
        var options = await _driver.GetSelectionRuleOptions(rule.RegistrationSelectionRuleId);

        // Assert
        options.Should().NotBeEmpty();
        options.Should().AllSatisfy(o =>
        {
            o.ElementId.Should().NotBe(Guid.Empty);
            o.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task RegisterSelectionRule_Adds_NewRegistration_For_Class()
    {
        // Arrange
        var rule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class);
        var options = await _driver.GetSelectionRuleOptions(rule.RegistrationSelectionRuleId);
        var chosenOption = options[0];

        // Act
        var newRegistrationId = await _driver.RegisterSelectionRule(rule, chosenOption);

        // Assert
        newRegistrationId.Should().NotBe(Guid.Empty);
        rule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class);
        rule.ActiveRegistration.Should().Be(chosenOption.ElementId, "Expected the selection rule to have an active registration after registering a class.");
    }
}
