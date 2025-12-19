using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class SpellsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private ElementsCreationDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _driver = _integration.GetDriver<ElementsCreationDriver>();
    }

    // get casting time list
    // get range list
    // get duration list

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task CreateSpell()
    {
        // Arrange
        const string name = "Fireball";
        const int level = 3;
        const string school = "Evocation";
        const string time = "1 action";
        const string range = "150 feet";
        const string duration = "Instantaneous";
        const bool isConcentration = false;
        const bool isRitual = false;
        const bool hasSomatic = true;
        const bool hasVerbal = true;
        const bool hasMaterial = true;
        const string materialComponent = "a piece of bat guano";
        string description = "A bright streak flashes from your pointing finger to a point you choose within range.";

        // Act
        var id = await _driver.CreateSpellAsync(name, level, school, time, range, duration, isConcentration, isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);

        // Assert
        id.Should().NotBeEmpty();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpellById()
    {
        // Arrange
        var fireballId = await _driver.CreateSpell(TestSpells.Fireball);

        // Act
        var spell = await _driver.GetSpellByIdAsync(fireballId);

        // Assert
        spell.Should().NotBeNull();
        spell.Id.Should().Be(fireballId);
        spell.Name.Should().Be("Fireball");
        spell.Level.Should().Be(3);
        spell.MagicSchool.Should().Be("Evocation");
        spell.CastingTime.Should().Be("1 action");
        spell.Range.Should().Be("150 feet");
        spell.Duration.Should().Be("Instantaneous");
        spell.IsConcentration.Should().Be(false);
        spell.IsRitual.Should().Be(false);
        spell.HasSomatic.Should().Be(true);
        spell.HasVerbal.Should().Be(true);
        spell.HasMaterial.Should().Be(true);
        spell.MaterialComponent.Should().Be("A tiny ball of bat guano and sulfur");
        spell.Description.Should().Be("A bright streak flashes from your pointing finger to a point you choose within range and then blossoms with a low roar into an explosion of flame.");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpellById_WhenMissing_ReturnsNull()
    {
        // Act
        var spell = await _driver.GetSpellByIdAsync(Guid.NewGuid());

        // Assert
        spell.Should().BeNull();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task UpdateSpell_WhenSpellExists_UpdatesSpellData()
    {
        // Arrange
        var fireballId = await _driver.CreateSpell(TestSpells.Fireball);

        var updateRequest = new UpdateSpellRequest
        {
            Id = fireballId,
            Level = 1,
            MagicSchool = "Transmutation",
            CastingTime = "1 bonus action",
            Range = "Self",
            Duration = "1 minute",
            IsConcentration = true,
            IsRitual = true,
            HasSomatic = false,
            HasVerbal = true,
            HasMaterial = false,
            MaterialComponent = null,
            Description = "Updated description"
        };

        // Act
        var updateResponse = await _driver.UpdateSpellAsync(updateRequest);

        // Assert
        var updated = await _driver.GetSpellByIdAsync(fireballId);
        updateResponse.Id.Should().Be(fireballId);

        updated.Should().NotBeNull();

        updated.Id.Should().Be(fireballId);
        updated.Name.Should().Be("Fireball", "spell name is immutable (until UpdateName on element is added)");
        updated.Level.Should().Be(updateRequest.Level);
        updated.MagicSchool.Should().Be(updateRequest.MagicSchool);
        updated.CastingTime.Should().Be(updateRequest.CastingTime);
        updated.Range.Should().Be(updateRequest.Range);
        updated.Duration.Should().Be(updateRequest.Duration);
        updated.IsConcentration.Should().Be(updateRequest.IsConcentration);
        updated.IsRitual.Should().Be(updateRequest.IsRitual);
        updated.HasSomatic.Should().Be(updateRequest.HasSomatic);
        updated.HasVerbal.Should().Be(updateRequest.HasVerbal);
        updated.HasMaterial.Should().Be(updateRequest.HasMaterial);
        updated.MaterialComponent.Should().BeNull();
        updated.Description.Should().Be(updateRequest.Description);
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpells_WhenNoneExist_ReturnsEmptyList()
    {
        // Act
        var payload = await _driver.GetSpellsAsync();

        // Assert
        payload.Items.Should().BeEmpty();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpells_WhenSpellsExist_ReturnsSpellSummariesWithoutDescription()
    {
        // Arrange
        var lightId = await _driver.CreateSpell(TestSpells.Light);
        var fireballId = await _driver.CreateSpell(TestSpells.Fireball);

        // Act
        var payload = await _driver.GetSpellsAsync();

        // Assert
        payload.Items.Should().HaveCount(2);
        payload.Items.Select(x => x.Id).Should().BeEquivalentTo([lightId, fireballId]);
        payload.Items.Select(x => x.Name).Should().BeEquivalentTo(["Light", "Fireball"]);
        payload.Items.Select(x => x.Level).Should().BeEquivalentTo([0, 3]);
    }
}
