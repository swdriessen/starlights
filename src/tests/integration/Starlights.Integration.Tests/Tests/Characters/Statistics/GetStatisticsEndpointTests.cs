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
        statistics.Should().Contain(s => s.GroupName == "character:level", "Expected character level statistic to be present");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Returns_AbilityScoreStatistics()
    {
        // Act
        var statistics = await _driver.GetStatistics();

        // Assert
        statistics.Should().Contain(s => s.GroupName == "str:score", "Expected strength score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "str:modifier", "Expected strength modifier statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "dex:score", "Expected dexterity score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "con:score", "Expected constitution score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "int:score", "Expected intelligence score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "wis:score", "Expected wisdom score statistic to be present");
        statistics.Should().Contain(s => s.GroupName == "cha:score", "Expected charisma score statistic to be present");
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
        proficiency!.Values.Should().NotBeEmpty("Expected proficiency to have value contributions");
        proficiency.TotalValue.Should().BeGreaterThan(0, "Expected proficiency bonus to be greater than zero");
    }
}
