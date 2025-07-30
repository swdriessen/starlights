using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Services;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Tests;

[TestClass]
public class ElementsModuleInitializationTests
{
    private readonly Mock<ILogger<ElementsModuleInitializer>> _loggerMock = new();
    private readonly Mock<IPersistence> _persistenceMock = new();
    private readonly Mock<IElementsRepository> _elementsRepositoryMock = new();

    // SUT
    private readonly ElementsModuleInitializer _initialization;

    public ElementsModuleInitializationTests()
    {
        _initialization = new ElementsModuleInitializer(_loggerMock.Object, _persistenceMock.Object);

        _persistenceMock.Setup(x => x.GetRepository<IElementsRepository>())
            .Returns(_elementsRepositoryMock.Object);
    }

    [TestMethod]
    public async Task NewElementsStored()
    {
        // Act
        var result = await _initialization.InitializeAsync();

        // Assert
        _elementsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Element>()), Times.AtLeastOnce);
        _persistenceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
