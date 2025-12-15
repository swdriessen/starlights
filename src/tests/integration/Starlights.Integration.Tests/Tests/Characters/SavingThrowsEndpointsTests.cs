using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class SavingThrowsEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private SavingThrowDriver _savingThrowsDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _savingThrowsDriver = _integration.GetDriver<SavingThrowDriver>();

        await _integration.InitializeElements();

        await _integration.GetDriver<CharacterCreationDriver>()
            .CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSavingThrows_Returns_Data()
    {
        // Act
        var savingThrows = await _savingThrowsDriver.GetSavingThrows();

        // Assert
        savingThrows.Should().HaveCount(6, "There should be exactly six saving throws, one for each ability score.");
        savingThrows.Should().AllSatisfy(save =>
        {
            save.SavingThrowId.Should().NotBe(Guid.Empty);
            save.Name.Should().NotBeNullOrWhiteSpace();
            save.AbilityScoreId.Should().NotBe(Guid.Empty);
            save.AbilityScoreAbbreviation.Should().NotBeNullOrWhiteSpace();
            save.CalculatedBonus.Should().Be(save.AbilityScoreModifier + save.AdditionalBonus);
        });
    }
}
