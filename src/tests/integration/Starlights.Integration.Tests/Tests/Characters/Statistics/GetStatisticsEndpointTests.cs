using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Statistics;

[TestClass]
public sealed class GetStatisticsEndpointTests : IntegrationTestBase
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
    public async Task GetStatistics_Returns_CalculatedStatistics()
    {
        // Act
        var statistics = await _driver.GetStatistics();

        // Assert
        statistics.Should().NotBeEmpty();
        statistics.Should().Contain(s => s.GroupName == "proficiency", "Expected proficiency statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "level", "Expected level statistic to be present");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Returns_AbilityScoreStatistics()
    {
        // Act
        var statistics = await _driver.GetStatistics();

        // Assert
        statistics.Should().Contain(s => s.GroupName == "strength:score", "Expected strength score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "strength:modifier", "Expected strength modifier statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "dexterity:score", "Expected dexterity score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "constitution:score", "Expected constitution score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "intelligence:score", "Expected intelligence score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "wisdom:score", "Expected wisdom score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "charisma:score", "Expected charisma score statistic to be present");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Returns_FinalizedGroups()
    {
        // Act
        var statistics = await _driver.GetStatistics();

        // Assert
        statistics.Should().AllSatisfy(s => s.IsFinalized.Should().BeTrue("All statistic groups should be finalized"));
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Includes_ValueBreakdown()
    {
        // Act
        var statistics = await _driver.GetStatistics();

        // Assert
        var proficiency = statistics.FirstOrDefault(s => s.GroupName == "proficiency");
        proficiency.Should().NotBeNull("Expected proficiency statistic to be present");
        proficiency.Values.Should().NotBeEmpty("Expected proficiency to have value contributions");
        proficiency.TotalValue.Should().BeGreaterThan(0, "Expected proficiency bonus to be greater than zero");
    }

    // get statitics for a level 5 barbarian
    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Level5Barbarian_HasCorrectProficiencyBonus()
    {
        // Arrange
        var reg = _integration.GetDriver<RegistrationDriver>();
        var characterDriver = _integration.GetDriver<CharacterManagementDriver>();
        //await characterDriver.UpdateCharacterAsync(updates =>
        //{
        //    updates.SetLevel(5);
        //    updates.SetClass("Barbarian");
        //});
        await reg.RegisterClass("Barbarian");
        await characterDriver.LevelUp("Barbarian", 5);


        // Act
        await Task.Delay(2000);
        var statistics = await _driver.GetStatistics();
        // Assert
        var proficiency = statistics.FirstOrDefault(s => s.GroupName == "proficiency");
        proficiency.Should().NotBeNull("Expected proficiency statistic to be present");
        proficiency!.TotalValue.Should().Be(3, "Expected proficiency bonus for level 5 character to be 3");
    }
}
