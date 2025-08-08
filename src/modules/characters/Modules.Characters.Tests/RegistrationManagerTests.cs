using FluentAssertions;
using Modules.Characters.Services.Processing;
using Moq;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class RegistrationManagerTests
{
    private readonly Mock<IPersistence> _persistence = new();
    private readonly Mock<IElementsModuleQueries> _elements = new();
    private readonly Mock<IRegistrationRepository> _registrations = new();

    private RegistrationManager CreateSut()
        => new(_persistence.Object, _elements.Object);

    [TestInitialize]
    public void Setup()
    {
        _persistence.Reset();
        _elements.Reset();
        _registrations.Reset();

        _persistence.Setup(p => p.GetRepository<IRegistrationRepository>())
                    .Returns(_registrations.Object);
        _persistence.Setup(p => p.SaveChangesAsync())
                    .ReturnsAsync(1);
    }

    [TestMethod]
    public async Task ProcessRegistration_Throws_WhenRegistrationNotFound()
    {
        // Arrange
        var sut = CreateSut();
        var registrationId = RegistrationId.New();
        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync((Registration?)null);

        // Act
        var action = async () => await sut.ProcessRegistration(registrationId);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Registration with ID {registrationId} not found.");

        _persistence.Verify(p => p.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task ProcessRegistration_AddsIncludedRegistrations_And_MarksProcessed()
    {
        // Arrange
        var sut = CreateSut();
        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // the current element has one include rule
        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(new ElementDataModel
                 {
                     Id = baseElementId,
                     Name = "Base",
                     Type = "Type",
                     IncludeRules = [new IncludeRuleDataModel(includeRuleId, includedElementId, 0)]
                 });

        // the included element resolved by the rule
        const string includedName = "Included-1";
        const string includedType = "IncludedType-1";
        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(new ElementDataModel
                 {
                     Id = includedElementId,
                     Name = includedName,
                     Type = includedType,
                     IncludeRules = []
                 });

        // capture added registration for assertions
        Registration? addedRegistration = null;
        _registrations.Setup(r => r.Add(It.IsAny<Registration>()))
                      .Callback<Registration>(reg => addedRegistration = reg);

        _persistence.Setup(p => p.SaveChangesAsync())
                    .ReturnsAsync(2);

        // Act
        var affected = await sut.ProcessRegistration(registrationId);

        // Assert
        affected.Should().Be(2);
        registration.IsProcessed.Should().BeTrue();
        registration.HasAssociatedRule(includeRuleId).Should().BeTrue();

        addedRegistration.Should().NotBeNull();
        addedRegistration!.ParentRegistrationId.Should().Be(registration.Id);
        addedRegistration.AssociatedElementId.Value.Should().Be(includedElementId);
        addedRegistration.AssociatedElementName.Should().Be(includedName);
        addedRegistration.AssociatedElementType.Should().Be(includedType);

        _registrations.Verify(r => r.Add(It.IsAny<Registration>()), Times.Once);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_SkipsWhenRuleAlreadyAssociated()
    {
        // Arrange
        var sut = CreateSut();
        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        // pre-associate rule so manager should skip it
        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(new ElementDataModel
                 {
                     Id = baseElementId,
                     Name = "Base",
                     IncludeRules = [new IncludeRuleDataModel(includeRuleId, includedElementId, 0)]
                 });

        // Act
        var affected = await sut.ProcessRegistration(registrationId);

        // Assert
        affected.Should().Be(1);
        registration.IsProcessed.Should().BeTrue();

        // No additional registrations added
        _registrations.Verify(r => r.Add(It.IsAny<Registration>()), Times.Never);

        // Ensure we did not fetch the included element because rule was already associated
        _elements.Verify(m => m.GetElementWithRules(includedElementId), Times.Never);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }
}
