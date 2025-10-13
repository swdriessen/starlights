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
    private readonly Mock<ICharactersRepository> _characters = new();

    private RegistrationProcessor CreateProcessor(params IRegistrationBehavior[] behaviors)
    {
        var manager = new RegistrationManager(Mock.Of<ILogger<RegistrationManager>>(), _persistence.Object, behaviors);
        return new(Mock.Of<ILogger<RegistrationProcessor>>(), _persistence.Object, manager, _elements.Object);
    }

    [TestInitialize]
    public void Setup()
    {
        _persistence.Reset();
        _elements.Reset();
        _registrations.Reset();
        _characters.Reset();

        _persistence.Setup(p => p.GetRepository<IRegistrationRepository>())
                    .Returns(_registrations.Object);
        _persistence.Setup(p => p.GetRepository<ICharactersRepository>())
                    .Returns(_characters.Object);
        _persistence.Setup(p => p.SaveChangesAsync())
                    .ReturnsAsync(1);
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldThrowAndNotSaveChanges_WhenRegistrationNotFound()
    {
        // Arrange
        var registrationId = RegistrationId.New();
        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync((Registration?)null);

        // Act
        var processor = CreateProcessor();
        var act = () => processor.ProcessRegistration(registrationId);
        await act.Should().ThrowAsync<RegistrationProcessingException>();

        // Assert
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldSaveChanges_WhenIncludeAddsRegistration()
    {
        // Arrange
        var processor = CreateProcessor();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        // Act
        _ = await processor.ProcessRegistration(registrationId);

        // Assert
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_ShouldCreateSelectionRules_WhenPresentOnElement()
    {
        // Arrange
        var processor = CreateProcessor();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        var selectionRuleId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithSelectionRules(baseElementId, selectionRuleId));

        // Act
        var result = await processor.ProcessRegistration(registrationId);

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
        var processor = CreateProcessor();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        var selectionRuleId = Guid.NewGuid();
        registration.CreateSelectionRule(new(selectionRuleId), "Skill", "Choose Skill");

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithSelectionRules(baseElementId, selectionRuleId));

        // Act
        var result = await processor.ProcessRegistration(registrationId);

        // Assert
        result.AffectedRows.Should().Be(1);
        registration.SelectionRules.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task ProcessRegistration_MarksRegistrationProcessed_WhenIncludeAddsRegistration()
    {
        // Arrange
        var processor = CreateProcessor();

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithoutRules(baseElementId));

        // Act
        _ = await processor.ProcessRegistration(registrationId);

        // Assert
        registration.IsProcessed.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_AssociatesIncludeRule_WhenIncludeAddsRegistration()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);

        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await processor.ProcessRegistration(registrationId);

        // Assert
        registration.HasAssociatedRule(includeRuleId).Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_AddsIncludedRegistrationWithExpectedProperties()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);

        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.Parse("11111111-0000-4000-0000-000000000000");
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.Parse("22222222-0000-4000-0000-000000000000");

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        Registration? added = null;
        _registrations.Setup(r => r.Add(It.IsAny<Registration>()))
                      .Callback<Registration>(reg => added = reg);

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await processor.ProcessRegistration(registrationId);

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
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);

        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");
        var registrationId = registration.Id;

        _registrations.Setup(r => r.GetRegistrationAsync(registrationId))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        _elements.Setup(m => m.GetElementWithRules(includedElementId))
                 .ReturnsAsync(CreateIncludedElement(includedElementId));

        _persistence.Setup(p => p.SaveChangesAsync()).ReturnsAsync(2);

        // Act
        _ = await processor.ProcessRegistration(registrationId);

        // Assert
        behavior.Verify(b => b.Registered(It.Is<Registration>(r => r.AssociatedElementId.Value == includedElementId && r.ParentRegistrationId == registration.Id)), Times.Once);
    }

    [TestMethod]
    public async Task ProcessRegistration_MarksRegistrationProcessed_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);
        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        var result = await processor.ProcessRegistration(registration.Id);

        // Assert
        result.AffectedRows.Should().Be(1);
        registration.IsProcessed.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessRegistration_DoesNotCreateOrInvoke_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);
        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        _ = await processor.ProcessRegistration(registration.Id);

        // Assert
        _registrations.Verify(r => r.Add(It.IsAny<Registration>()), Times.Never);
        behavior.Verify(b => b.Registered(It.IsAny<Registration>()), Times.Never);
    }

    [TestMethod]
    public async Task ProcessRegistration_DoesNotFetchIncludedElement_WhenRuleAlreadyAssociated()
    {
        // Arrange
        var behavior = new Mock<IRegistrationBehavior>();
        behavior.Setup(b => b.Registered(It.IsAny<Registration>()))
                .Returns(Task.CompletedTask);
        var processor = CreateProcessor(behavior.Object);

        var characterId = CharacterId.New();
        var baseElementId = Guid.NewGuid();
        var registration = Registration.Create(characterId, new(baseElementId), "Base", "Type");

        var includeRuleId = Guid.NewGuid();
        var includedElementId = Guid.NewGuid();

        registration.CreateIncludeRule(new(includeRuleId), new(includedElementId), "Pre-Added");

        _registrations.Setup(r => r.GetRegistrationAsync(registration.Id))
                      .ReturnsAsync(registration);

        // character lookup
        var character = Character.Create("Test");
        _characters.Setup(c => c.GetCharacterAsync(characterId.Value))
                   .ReturnsAsync(character);

        _elements.Setup(m => m.GetElementWithRules(baseElementId))
                 .ReturnsAsync(CreateBaseElementWithIncludeRule(baseElementId, includeRuleId, includedElementId));

        // Act
        _ = await processor.ProcessRegistration(registration.Id);

        // Assert
        _elements.Verify(m => m.GetElementWithRules(includedElementId), Times.Never);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    private static ElementDataModel CreateBaseElementWithIncludeRule(Guid baseElementId, Guid includeRuleId, Guid includedElementId, string name = "Base", string type = "Type", int levelRequirement = 0)
    {
        return new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            IncludeRules = [new IncludeRuleDataModel(includeRuleId, includedElementId, levelRequirement)]
        };
    }

    private static ElementDataModel CreateBaseElementWithSelectionRules(Guid baseElementId, Guid selectionRuleId, string name = "Base", string type = "Type")
    {
        return new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            SelectionRules = [new SelectionRuleDataModel(selectionRuleId, "Skill", "Choose Skill", 0)]
        };
    }

    private static ElementDataModel CreateIncludedElement(Guid includedElementId, string name = "Included-1", string type = "IncludedType-1")
    {
        return new()
        {
            Id = includedElementId,
            Name = name,
            Type = type,
            IncludeRules = []
        };
    }

    private static ElementDataModel CreateBaseElementWithoutRules(Guid baseElementId, string name = "Base", string type = "Type")
    {
        return new()
        {
            Id = baseElementId,
            Name = name,
            Type = type,
            IncludeRules = []
        };
    }
}
