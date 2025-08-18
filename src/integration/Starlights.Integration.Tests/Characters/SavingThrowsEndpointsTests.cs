using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.Entities.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class SavingThrowsEndpointsTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public SavingThrowsEndpointsTests()
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

        var optionsResponse = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        optionsResponse.Options.Should().NotBeEmpty();

        var portraitsResponse = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);
        portraitsResponse.Portraits.Should().NotBeEmpty();

        var name = $"Integration Test Character {Guid.NewGuid()}";
        var characterResponse = await client.CreateCharacterAsync(optionsResponse.Options[0].Id, name, portraitsResponse.Portraits[0].Url, TestCancellationToken);
        _integration.SetCharacterIdentifier(characterResponse.Id);

        await _eventListener.SavingThrowCreated.WaitForEvent(count: 6, cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.TimeoutForDebugging, CooperativeCancellation = true)]
    public async Task GetSavingThrows_Returns_Data()
    {
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/characters/{characterId}/savingthrows", TestCancellationToken);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var saves = await response.Content.ReadFromJsonAsync<GetSavingThrowsResponse>();

        saves.Should().NotBeNull();
        saves.SavingThrows.Should().NotBeEmpty();

        var save = saves.SavingThrows[0];
        save.SavingThrowId.Should().NotBe(Guid.Empty);
        save.Name.Should().NotBeNullOrWhiteSpace();
        save.AbilityScoreId.Should().NotBe(Guid.Empty);
        save.AbilityScoreAbbreviation.Should().NotBeNullOrWhiteSpace();
        save.CalculatedBonus.Should().Be(save.AbilityScoreModifier + save.AdditionalBonus);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetSavingThrows_UnknownCharacter_Returns_NotFound()
    {
        var client = _integration.CreateClient();
        var unknownCharacterId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/characters/{unknownCharacterId}/savingthrows", TestCancellationToken);
        await response.ShouldHaveStatusAsync(System.Net.HttpStatusCode.NotFound);
    }
}
