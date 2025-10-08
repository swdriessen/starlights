using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Modules.Elements.Integration.Models.Rules;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Tests.Registrations;

[TestClass]
public sealed class RegistrationProcessorTests
{
    private readonly Mock<IPersistence> _persistence = new();
    private readonly Mock<IElementsModuleQueries> _elements = new();
    private readonly Mock<IRegistrationRepository> _registrations = new();

    private RegistrationProcessor CreateSut(params IRegistrationBehavior[] behaviors)
    {
        var manager = new NewRegistrationManager(Mock.Of<ILogger<NewRegistrationManager>>(), _persistence.Object, behaviors);
        return new(Mock.Of<ILogger<RegistrationProcessor>>(), _persistence.Object, _elements.Object, manager);
    }

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
    public async Task ProcessRegistration_ShouldNotSaveChanges_WhenRegistrationNotFound()
    {
        // Arrange
        var sut = CreateSut();
        var registrationId = RegistrationId.New();
        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync((Registration?)null);

        // Act
        await sut.ProcessRegistration(registrationId);

        // Assert
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldSaveChanges_WhenIncludeAddsRegistration()
    {
        // Arrange
        var sut = CreateSut();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        // Act
        _ = await sut.ProcessRegistration(registrationId);

        // Assert
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldCreateSelectionRules_WhenPresentOnElement()
    {
        // Arrange
        var sut = CreateSut();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        var selectionRuleId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithSelectionRules(baseElementId, selectionRuleId));

        // Act
        var result = await sut.ProcessRegistration(registrationId);

        // Assert
        result.AffectedRows.Should().Be(1);
        registration.SelectionRules.Should().HaveCount(1);
        registration.HasAssociatedRule(selectionRuleId).Should().BeTrue();
        var applied = registration.SelectionRules.Single();
        applied.ElementType.Should().Be("Skill");
        applied.Name.Should().Be("Choose Skill");
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldNotDuplicateSelectionRules_WhenAlreadyAssociated()
    {
        // Arrange
        var sut = CreateSut();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        var selectionRuleId = Guid.NewGuid();
        registration.CreateSelectionRule(new(selectionRuleId), "Skill", "Choose Skill");

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithSelectionRules(baseElementId, selectionRuleId));

        // Act
        var result = await sut.ProcessRegistration(registrationId);

        // Assert
        result.AffectedRows.Should().Be(1);
        registration.SelectionRules.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task ProcessRegistration_MarksRegistrationProcessed_WhenIncludeAddsRegistration()
    {
        // Arrange
        var sut = CreateSut();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithoutRules(baseElementId));

        // Act
        _ = await sut.ProcessRegistration(registrationId);

        // Assert
        registration.IsProcessed.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_AssociatesIncludeRule_WhenIncludeAddsRegistration()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);

        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await sut.ProcessRegistration(registrationId);

        // Assert
        registration.HasAssociatedRule(includeRuleId).Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_AddsIncludedRegistrationWithExpectedProperties()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);

        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        Registration? added = null;
        _registrations.Setup(r => r.Add(It.IsAny<Registration>()))
                      .Callback<Registration>(reg => added = reg);

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await sut.ProcessRegistration(registrationId);

        // Assert
        added.Should().NotBeNull();
        added!.ParentRegistrationId.Should().Be(registration.Id);
        added.AssociatedElementId.Value.Should().Be(includedElementId);
        added.AssociatedElementName.Should().Be("Included-1");
        added.AssociatedElementType.Should().Be("IncludedType-1");
        _registrations.Verify(r => r.Add(It.IsAny<Registration>()), Times.Once);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_InvokesBehaviors_ForIncludedRegistration()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);

        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await sut.ProcessRegistration(registrationId);

        // Assert
        behavior.Verify(b => b.Registered(
            It.Is<Registration>(r => r.AssociatedElementId.Value == includedElementId && r.ParentRegistrationId == registration.Id),
            It.IsAny<RegistrationProcessContext>()),
            Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_MarksRegistrationProcessed_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);
        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        var result = await sut.ProcessRegistration(registration.Id);

        // Assert
        result.AffectedRows.Should().Be(1);
        registration.IsProcessed.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_DoesNotCreateOrInvoke_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);
        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        _ = await sut.ProcessRegistration(registration.Id);

        // Assert
        _registrations.Verify(r => r.Add(It.IsAny<Registration>()), Times.Never);
        behavior.Verify(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()), Times.Never);
    }

    [TestMethod]
    public async Task ProcessRegistration_DoesNotFetchIncludedElement_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>(), It.IsAny<RegistrationProcessContext>()))
                .Returns(Task.CompletedTask);
        var sut = CreateSut(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        _ = await sut.ProcessRegistration(registration.Id);

        // Assert
        _elements.Verify(m => m.GetElementWithRules(includedElementId), Times.Never);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    private static ElementDataModel CreateBaseElementWithIncludeRule(Guid baseElementId, Guid includeRuleId, Guid includedElementId, string name = "Base", string type = "Type", int levelRequirement = 0)
        => new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            IncludeRules = [new IncludeRuleDataModel(includeRuleId, includedElementId, levelRequirement)]
        };

    private static ElementDataModel CreateBaseElementWithSelectionRules(Guid baseElementId, Guid selectionRuleId, string name = "Base", string type = "Type")
        => new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            SelectionRules = [new SelectionRuleDataModel(selectionRuleId, "Skill", "Choose Skill", 0)]
        };

    private static ElementDataModel CreateIncludedElement(Guid includedElementId, string name = "Included-1", string type = "IncludedType-1")
        => new()
        {
            Id = includedElementId,
            Name = name,
            Type = type,
            IncludeRules = []
        };

    private static ElementDataModel CreateBaseElementWithoutRules(Guid baseElementId, string name = "Base", string type = "Type")
        => new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            IncludeRules = []
        };
}
