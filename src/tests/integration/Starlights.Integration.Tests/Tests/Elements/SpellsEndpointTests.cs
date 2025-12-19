using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.Elements;
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
    public async Task CreateCantrip()
    {
        // Arrange
        const string name = "Firebolt";
        const int level = 0;
        const string school = "Evocation";
        const string time = "1 action";
        const string range = "120 feet";
        const string duration = "Instantaneous";

        const bool isConcentration = false;
        const bool isRitual = false;

        const bool hasSomatic = true;
        const bool hasVerbal = true;
        const bool hasMaterial = true;
        const string materialComponent = "a piece of bat guano";



        //string[] classes = new[] { "Sorcerer", "Wizard" };


        string description = "A bright streak flashes from your pointing finger to a point you choose within range.";


        // Act
        var id = await _driver.CreateSpellAsync(name, level, school, time, range, duration, isConcentration, isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);

        // Assert
        id.Should().NotBeEmpty();
    }

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



        //string[] classes = new[] { "Sorcerer", "Wizard" };


        string description = "A bright streak flashes from your pointing finger to a point you choose within range.";


        // Act
        var id = await _driver.CreateSpellAsync(name, level, school, time, range, duration, isConcentration, isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);

        // Assert
        id.Should().NotBeEmpty();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSpellById_WhenSpellExists_ReturnsSpellData()
    {
        // Arrange
        const string name = "Firebolt";
        const int level = 0;
        const string school = "Evocation";
        const string time = "1 action";
        const string range = "120 feet";
        const string duration = "Instantaneous";

        const bool isConcentration = false;
        const bool isRitual = false;

        const bool hasSomatic = true;
        const bool hasVerbal = true;
        const bool hasMaterial = true;
        const string materialComponent = "a piece of bat guano";

        const string description = "A bright streak flashes from your pointing finger to a point you choose within range.";

        var id = await _driver.CreateSpellAsync(name, level, school, time, range, duration, isConcentration, isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);

        // Act
        var spell = await _driver.GetSpellByIdAsync(id);

        // Assert
        spell.Should().NotBeNull();
        spell!.Id.Should().Be(id);
        spell.Name.Should().Be(name);
        spell.Level.Should().Be(level);
        spell.MagicSchool.Should().Be(school);
        spell.CastingTime.Should().Be(time);
        spell.Range.Should().Be(range);
        spell.Duration.Should().Be(duration);
        spell.IsConcentration.Should().Be(isConcentration);
        spell.IsRitual.Should().Be(isRitual);
        spell.HasSomatic.Should().Be(hasSomatic);
        spell.HasVerbal.Should().Be(hasVerbal);
        spell.HasMaterial.Should().Be(hasMaterial);
        spell.MaterialComponent.Should().Be(materialComponent);
        spell.Description.Should().Be(description);
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
        const string name = "Firebolt";
        const int level = 0;
        const string school = "Evocation";
        const string time = "1 action";
        const string range = "120 feet";
        const string duration = "Instantaneous";

        const bool isConcentration = false;
        const bool isRitual = false;

        const bool hasSomatic = true;
        const bool hasVerbal = true;
        const bool hasMaterial = true;
        const string materialComponent = "a piece of bat guano";

        const string description = "A bright streak flashes from your pointing finger to a point you choose within range.";

        var id = await _driver.CreateSpellAsync(name, level, school, time, range, duration, isConcentration, isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);

        var updateRequest = new UpdateSpellRequest
        {
            Id = id,
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
        var updated = await _driver.GetSpellByIdAsync(id);

        // Assert
        updateResponse.Id.Should().Be(id);
        updated.Should().NotBeNull();
        updated!.Id.Should().Be(id);
        updated.Name.Should().Be(name, "spell name is immutable");
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
}
