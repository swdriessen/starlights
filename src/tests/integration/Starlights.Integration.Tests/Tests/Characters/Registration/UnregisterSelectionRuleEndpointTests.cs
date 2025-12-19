using AwesomeAssertions;
using Starlights.Integration.Constants;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Extensions;

namespace Starlights.Integration.Tests.Characters.Registration;

[TestClass]
public sealed class UnregisterSelectionRuleEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private RegistrationDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();

        _driver = _integration.GetDriver<RegistrationDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
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
}