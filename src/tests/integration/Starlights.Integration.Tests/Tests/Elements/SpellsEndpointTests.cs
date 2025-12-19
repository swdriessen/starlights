using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Extensions;
using Starlights.Integration.Extensions;
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
        _integration = IntegrationHost.CreateDefaultBuilder(this)
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
        var properties = new CreateSpellProperties()
        {
            Name = "Fireball",
            Level = 3,
            School = "Evocation",
            Time = "1 action",
            Range = "150 feet",
            Duration = "Instantaneous",
            IsConcentration = false,
            IsRitual = false,
            HasSomatic = true,
            HasVerbal = true,
            HasMaterial = true,
            MaterialComponent = "a piece of bat guano",
            Description = "A bright streak flashes from your pointing finger to a point you choose within range."
        };

        // Act
        var id = await _driver.CreateSpellAsync(properties);

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
        spell.IsConcentration.Should().BeFalse();
        spell.IsRitual.Should().BeFalse();
        spell.HasSomatic.Should().BeTrue();
        spell.HasVerbal.Should().BeTrue();
        spell.HasMaterial.Should().BeTrue();
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
        updateResponse.Id.Should().Be(fireballId);

        var updated = await _driver.GetSpellByIdAsync(fireballId);
        updated.Should().NotBeNull();
        updated.Id.Should().Be(fireballId);
        updated.Name.Should().Be("Fireball", "spell name is immutable (until UpdateName on element is added)");
        updated.Level.Should().Be(updateRequest.Level);
        updated.MagicSchool.Should().Be(updateRequest.MagicSchool);
        updated.CastingTime.Should().Be(updateRequest.CastingTime);
        updated.Range.Should().Be(updateRequest.Range);
        updated.Duration.Should().Be(updateRequest.Duration);
        updated.IsConcentration.Should().BeTrue();
        updated.IsRitual.Should().BeTrue();
        updated.HasSomatic.Should().BeFalse();
        updated.HasVerbal.Should().BeTrue();
        updated.HasMaterial.Should().BeFalse();
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
