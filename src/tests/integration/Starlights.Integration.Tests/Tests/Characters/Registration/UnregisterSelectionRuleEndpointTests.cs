using FluentAssertions;
using Starlights.Integration.Constants;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Registration;

[TestClass]
public sealed class UnregisterSelectionRuleEndpointTests : IntegrationTestBase
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
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task UnregisterSelectionRule_Removes_ExistingRegistration_For_Class()
    {
        // Arrange
        var (classRule, barbarianOption, _) = await _driver.RegisterClass("Barbarian");
        classRule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class); // get the rule again to verify the active registration
        classRule.ActiveRegistration.Should().Be(barbarianOption.ElementId, "Expected the selection rule to have an active registration after registering a class.");

        // Act
        await _driver.UnregisterSelectionRule(classRule, barbarianOption);

        // Assert
        classRule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class);
        classRule.ActiveRegistration.Should().BeNull();
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task UnregisterSelectionRule_Removes_ExistingRegistration_For_Class2()
    {
        // Arrange
        var (rule, barbarianOption, _) = await _driver.RegisterClass("Barbarian");
        rule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class); // get the rule again to verify the active registration
        rule.ActiveRegistration.Should().Be(barbarianOption.ElementId, "Expected the selection rule to have an active registration after registering a class.");

        // Act
        await _driver.UnregisterClass(barbarianOption.Name);

        // Assert
        rule = await _driver.GetSingleSelectionRule(SelectionRuleTypes.Class);
        rule.ActiveRegistration.Should().BeNull();
    }
}