using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration.Abstractions.Models;
using Starlights.Modules.Elements.Services;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Tests;

[TestClass]
public class ElementsModuleQueriesTests
{
    private readonly Mock<ILogger<ElementsModuleQueries>> _loggerMock = new();
    private readonly Mock<IPersistence> _persistenceMock = new();
    private readonly Mock<IElementsRepository> _elementsRepositoryMock = new();

    // SUT
    private readonly ElementsModuleQueries _queries;

    public ElementsModuleQueriesTests()
    {
        _queries = new ElementsModuleQueries(_loggerMock.Object, _persistenceMock.Object);

        _persistenceMock.Setup(x => x.GetRepository<IElementsRepository>())
            .Returns(_elementsRepositoryMock.Object);

        // empty collection by default
        _elementsRepositoryMock.Setup(x => x.GetElementsByTypeAsync(ElementTypeConstants.CharacterCreation))
            .ReturnsAsync([]);

        _elementsRepositoryMock.Setup(x => x.GetElementAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Element?)null);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // save never callled in queries
        _persistenceMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task GetCharacterCreationElements_ReturnsEmpty_WhenNoElements()
    {
        // Act
        var result = await _queries.GetCharacterCreationElements();

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetCharacterCreationElements_ReturnsMappedElements()
    {
        // Arrange
        var element1 = Element.Create("Default Character", ElementTypeConstants.CharacterCreation);
        element1.AddComponent(new ShortDescriptionComponent(element1.Id, "a default description"));
        var element2 = Element.Create("Another Character", ElementTypeConstants.CharacterCreation);
        element2.AddComponent(new ShortDescriptionComponent(element2.Id, "another description"));

        var elements = new List<Element> { element1, element2 };

        _elementsRepositoryMock.Setup(x => x.GetElementsByTypeAsync(ElementTypeConstants.CharacterCreation))
            .ReturnsAsync(elements);

        // Act
        var result = await _queries.GetCharacterCreationElements();

        // Assert
        result.Should().ContainSingle(el =>
            el.Name == "Default Character" &&
            el.ShortDescription == "a default description"
        );
    }

    [TestMethod]
    public async Task GetCharacterCreationElement_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _queries.GetCharacterCreationElement(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetCharacterCreationElement_ReturnsMappedElement()
    {
        // Arrange
        var element = Element.Create("Custom", ElementTypeConstants.CharacterCreation);
        element.AddComponent(new ShortDescriptionComponent(element.Id, "With Description"));
        _elementsRepositoryMock.Setup(x => x.GetElementAsync(element.Id))
            .ReturnsAsync(element);

        // Act
        var result = await _queries.GetCharacterCreationElement(element.Id);

        // Assert
        result.Should().BeEquivalentTo(new CharacterCreationInfo(element.Id, "Custom") { ShortDescription = "With Description" });
    }

    [TestMethod]
    public async Task GetCharacterCreationElement_ReturnsElementWithoutDescription()
    {
        // Arrange
        var element = Element.Create("Custom", ElementTypeConstants.CharacterCreation);
        _elementsRepositoryMock.Setup(x => x.GetElementAsync(element.Id))
            .ReturnsAsync(element);

        // Act
        var result = await _queries.GetCharacterCreationElement(element.Id);

        // Assert
        result.Should().BeEquivalentTo(new CharacterCreationInfo(element.Id, "Custom"));
    }
}
