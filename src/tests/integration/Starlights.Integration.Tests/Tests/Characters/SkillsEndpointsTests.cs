using AwesomeAssertions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Extensions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class SkillsEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private SkillsDriver _skillsDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();

        _skillsDriver = _integration.GetDriver<SkillsDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetSkills_Returns_SkillData()
    {
        // Act
        var skills = await _skillsDriver.GetSkills();

        // Assert
        skills.Should().HaveCount(18, "There should be exactly eighteen skills for a new character.");
        skills.Should().AllSatisfy(s =>
        {
            s.SkillId.Should().NotBe(Guid.Empty);
            s.Name.Should().NotBeNullOrWhiteSpace();
            s.AbilityScoreId.Should().NotBe(Guid.Empty);
            s.AbilityScoreAbbreviation.Should().NotBeNullOrWhiteSpace();
            s.CalculatedBonus.Should().Be(s.AbilityScoreModifier + s.AdditionalBonus);
        });
    }
}
