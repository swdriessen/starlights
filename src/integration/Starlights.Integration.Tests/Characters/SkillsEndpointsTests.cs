using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class SkillsEndpointsTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public SkillsEndpointsTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();

        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.InitializeElementsAsync();

        // Create a character first
        var optionsResponse = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        optionsResponse.Options.Should().NotBeEmpty();

        var portraitsResponse = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);
        portraitsResponse.Portraits.Should().NotBeEmpty();

        var name = $"Integration Test Character {Guid.NewGuid()}";
        var characterResponse = await client.CreateCharacterAsync(optionsResponse.Options[0].Id, name, portraitsResponse.Portraits[0].Url, TestCancellationToken);
        _integration.SetCharacterIdentifier(characterResponse.Id);

        await _eventListener.SkillCreated.WaitForEvent(cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetSkills_Returns_SkillData()
    {
        // Arrange
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();

        // Act
        var skills = await client.GetSkillsAsync(characterId, TestCancellationToken);

        // Assert
        skills.Should().NotBeNull();
        skills.Skills.Should().NotBeEmpty();

        var first = skills.Skills[0];
        first.SkillId.Should().NotBe(Guid.Empty);
        first.Name.Should().NotBeNullOrWhiteSpace();
        first.AbilityScoreId.Should().NotBe(Guid.Empty);
        first.AbilityScoreAbbreviation.Should().NotBeNullOrWhiteSpace();
        first.CalculatedBonus.Should().Be(first.AbilityScoreModifier + first.AdditionalBonus);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetSkills_UnknownCharacter_Returns_NotFound()
    {
        // Arrange
        var unknownCharacterId = Guid.NewGuid();
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/characters/{unknownCharacterId}/skills", TestCancellationToken);

        // Assert
        await response.ShouldHaveStatusAsync(System.Net.HttpStatusCode.NotFound);
    }
}
