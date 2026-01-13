using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Extensions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells;
using static Starlights.Integration.Drivers.Elements.ManageSpellsDriver;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class SpellsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private ManageSpellsDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();

        //_integration.Set(new ElementsScenarioContext());

        _driver = _integration.GetDriver<ManageSpellsDriver>();
    }

    // get casting time list
    // get range list
    // get duration list

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task CreateSpell()
    {
        // Arrange
        var properties = new CreateProperties()
        {
            Name = "Fireball",
            Level = 3,
            MagicSchool = "Evocation",
            CastingTime = "1 action",
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
        var id = await _driver.CreateSpell(properties);

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
        var spell = await _driver.GetSpell(fireballId);

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
        var spell = await _driver.GetSpell(Guid.NewGuid());

        // Assert
        spell.Should().BeNull();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task UpdateSpell_WhenSpellExists_UpdatesSpellData()
    {
        // Arrange
        var fireballId = await _driver.CreateSpell(TestSpells.Fireball);

        var updateRequest = new SpellDataModel
        {
            Id = fireballId,
            Name = "Fireball",
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
        await _driver.UpdateSpell(updateRequest);

        // Assert
        var updated = await _driver.GetSpell(fireballId);
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
        var spells = await _driver.GetSpells();

        // Assert
        spells.Should().BeEmpty();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpells_WhenSpellsExist_ReturnsSpellSummariesWithoutDescription()
    {
        // Arrange
        var lightId = await _driver.CreateSpell(TestSpells.Light);
        var fireballId = await _driver.CreateSpell(TestSpells.Fireball);

        // Act
        var spells = await _driver.GetSpells();

        // Assert
        spells.Should().HaveCount(2);
        spells.Select(x => x.Id).Should().BeEquivalentTo([lightId, fireballId]);
        spells.Select(x => x.Name).Should().BeEquivalentTo(["Light", "Fireball"]);
        spells.Select(x => x.Level).Should().BeEquivalentTo([0, 3]);
    }
}
