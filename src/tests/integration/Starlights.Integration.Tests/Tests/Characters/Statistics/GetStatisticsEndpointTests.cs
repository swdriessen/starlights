using AwesomeAssertions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;

namespace Starlights.Integration.Tests.Characters.Statistics;

[TestClass]
public sealed class GetStatisticsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private EventObserverCollection _events = default!;
    private RegistrationDriver _registration = default!;
    private CharacterManagementDriver _characterManagementDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();

        _events = _integration.GetEventObserverCollection();
        _registration = _integration.GetDriver<RegistrationDriver>();
        _characterManagementDriver = _integration.GetDriver<CharacterManagementDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Returns_CalculatedStatistics()
    {
        // Act
        var statistics = await _registration.GetStatistics();

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
        var statistics = await _registration.GetStatistics();

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
        var statistics = await _registration.GetStatistics();

        // Assert
        statistics.Should().AllSatisfy(s => s.IsFinalized.Should().BeTrue("All statistic groups should be finalized"));
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_Includes_ValueBreakdown()
    {
        // Act
        var statistics = await _registration.GetStatistics();

        // Assert
        var proficiency = statistics.FirstOrDefault(s => s.GroupName == "proficiency");
        proficiency.Should().NotBeNull("Expected proficiency statistic to be present");
        proficiency.Values.Should().NotBeEmpty("Expected proficiency to have value contributions");
        proficiency.TotalValue.Should().BeGreaterThan(0, "Expected proficiency bonus to be greater than zero");
    }

    [DataRow(1, 2)]
    [DataRow(4, 2)]
    [DataRow(5, 3)]
    [DataRow(8, 3)]
    [DataRow(9, 4)]
    [DataRow(12, 4)]
    [DataRow(13, 5)]
    [DataRow(16, 5)]
    [DataRow(17, 6)]
    [DataRow(20, 6)]
    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetStatistics_HasCorrectProficiencyBonus_WhenLevelingUp(int level, int expectedProficiencyValue)
    {
        // Arrange
        await _registration.RegisterClass("Barbarian");
        await _characterManagementDriver.LevelUp("Barbarian", level);
        await _events.EnsureObservation<RegistrationStatisticRuleCreatedEvent>(e => e.Name == "proficiency" && e.Value == $"{expectedProficiencyValue}");

        // Act
        var statistics = await _registration.GetStatistics();

        // Assert
        statistics.Should().NotBeEmpty();
        var proficiency = statistics.SingleOrDefault(s => s.GroupName == "proficiency");
        proficiency.Should().NotBeNull("Expected proficiency statistic to be present");
        proficiency.TotalValue.Should().Be(expectedProficiencyValue, $"Expected proficiency bonus for level {level} character to be {expectedProficiencyValue}");
    }
}
