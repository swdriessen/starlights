using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.Elements;

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
}
