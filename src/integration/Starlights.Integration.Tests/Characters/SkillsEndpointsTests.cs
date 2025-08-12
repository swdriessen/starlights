using FluentAssertions;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class SkillsEndpointsTests
{
    private readonly IntegrationHost _integration;
    private Guid _characterId;

    public SkillsEndpointsTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.InitializeElementsAsync();

        // Create a character first
        var optionsResponse = await client.GetCharacterCreationOptionsAsync(CancellationToken.None);
        optionsResponse.Options.Should().NotBeEmpty();

        var portraitsResponse = await client.GetCharacterPortraitOptionsAsync(CancellationToken.None);
        portraitsResponse.Portraits.Should().NotBeEmpty();

        var name = $"Integration Test Character {Guid.NewGuid()}";
        var characterResponse = await client.CreateCharacterAsync(optionsResponse.Options[0].Id, name, portraitsResponse.Portraits[0].Url, CancellationToken.None);
        _characterId = characterResponse.Id;

        // wait for skills to be created by background processing
        await client.WaitForSkillsAsync(_characterId, minCount: 1, timeout: TimeSpan.FromMilliseconds(3000), CancellationToken.None);
    }

    [TestMethod]
    public async Task GetSkills_Returns_SkillData()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var skills = await client.GetSkillsAsync(_characterId, CancellationToken.None);

        // Assert
        skills.Should().NotBeNull();
        skills.Skills.Should().NotBeEmpty();

        var first = skills.Skills[0];
        first.SkillId.Should().NotBe(Guid.Empty);
        first.Name.Should().NotBeNullOrWhiteSpace();
        // Ability may be assigned later by handlers; allow empty Guid early on
        first.CalculatedBonus.Should().Be(first.AbilityScoreModifier + first.AdditionalBonus);
    }

    [TestMethod]
    public async Task GetSkills_UnknownCharacter_Returns_NotFound()
    {
        // Arrange
        var client = _integration.CreateClient();
        var unknownId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/characters/{unknownId}/skills", CancellationToken.None);

        // Assert
        await response.ShouldHaveStatusAsync(System.Net.HttpStatusCode.NotFound);
    }
}
