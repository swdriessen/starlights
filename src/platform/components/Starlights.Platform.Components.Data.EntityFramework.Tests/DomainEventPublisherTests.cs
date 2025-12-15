using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Starlights.Platform.Eventing;
using Starlights.Platform.Eventing.EventPublisher;

namespace Starlights.Platform.Components.Data.EntityFramework.Tests;

[TestClass]
public sealed class DomainEventPublisherTests
{
    private ServiceCollection _services = null!;
    private IServiceProvider _serviceProvider = null!;
    private DomainEventPublisher _publisher = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();
        _serviceProvider = _services.BuildServiceProvider();
        // Default publisher for tests that don't assert handler calls; scope factory can be no-op
        var scopeFactory = Mock.Of<IServiceScopeFactory>(f => f.CreateScope() == Mock.Of<IServiceScope>(s => s.ServiceProvider == _serviceProvider));
        _publisher = new DomainEventPublisher(scopeFactory);
    }

    [TestMethod]
    public void Constructor_ShouldCreateInstance_WhenServiceProviderIsProvided()
    {
        // Arrange
        var scopeFactory = Mock.Of<IServiceScopeFactory>(f => f.CreateScope() == Mock.Of<IServiceScope>(s => s.ServiceProvider == Mock.Of<IServiceProvider>()));

        // Act
        var publisher = new DomainEventPublisher(scopeFactory);

        // Assert
        publisher.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PublishAsync_SingleEvent_ShouldInvokeHandler_WhenHandlerExists()
    {
        // Arrange
        var domainEvent = new TestDomainEvent("Test message");
        var handlerMock = new Mock<IDomainEventHandler<TestDomainEvent>>();

        // Setup a scoped provider that returns IEnumerable<IDomainEventHandler<TestDomainEvent>>
        var scopedProviderMock = new Mock<IServiceProvider>();
        var enumerableType = typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>);
        scopedProviderMock
            .Setup(p => p.GetService(enumerableType))
            .Returns(new[] { handlerMock.Object });

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.SetupGet(s => s.ServiceProvider).Returns(scopedProviderMock.Object);
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);
        var publisher = new DomainEventPublisher(scopeFactoryMock.Object);

        // Act
        await publisher.PublishAsync(domainEvent);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(domainEvent), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_SingleEvent_ShouldInvokeAllHandlers_WhenMultipleHandlersExist()
    {
        // Arrange
        var domainEvent = new TestDomainEvent("Test message");
        var handlerMock1 = new Mock<IDomainEventHandler<TestDomainEvent>>();
        var handlerMock2 = new Mock<IDomainEventHandler<TestDomainEvent>>();

        var scopedProviderMock = new Mock<IServiceProvider>();
        var enumerableType = typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>);
        scopedProviderMock
            .Setup(p => p.GetService(enumerableType))
            .Returns(new IDomainEventHandler<TestDomainEvent>[] { handlerMock1.Object, handlerMock2.Object });

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.SetupGet(s => s.ServiceProvider).Returns(scopedProviderMock.Object);
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);
        var publisher = new DomainEventPublisher(scopeFactoryMock.Object);

        // Act
        await publisher.PublishAsync(domainEvent);

        // Assert
        handlerMock1.Verify(h => h.HandleAsync(domainEvent), Times.Once);
        handlerMock2.Verify(h => h.HandleAsync(domainEvent), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_SingleEvent_ShouldCompleteSuccessfully_WhenNoHandlersExist()
    {
        // Arrange
        var domainEvent = new TestDomainEvent("Test message");

        var scopedProviderMock = new Mock<IServiceProvider>();
        var enumerableType = typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>);
        scopedProviderMock
            .Setup(p => p.GetService(enumerableType))
            .Returns(Array.Empty<IDomainEventHandler<TestDomainEvent>>());

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.SetupGet(s => s.ServiceProvider).Returns(scopedProviderMock.Object);
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);
        var publisher = new DomainEventPublisher(scopeFactoryMock.Object);

        // Act & Assert
        var act = async () => await publisher.PublishAsync(domainEvent);
        await act.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task PublishAsync_MultipleEvents_ShouldInvokeHandlersForAllEvents()
    {
        // Arrange
        var event1 = new TestDomainEvent("Event 1");
        var event2 = new TestDomainEvent("Event 2");
        var events = new List<IDomainEvent> { event1, event2 };

        var handlerMock = new Mock<IDomainEventHandler<TestDomainEvent>>();

        var scopedProviderMock = new Mock<IServiceProvider>();
        var enumerableType = typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>);
        scopedProviderMock
            .Setup(p => p.GetService(enumerableType))
            .Returns(new[] { handlerMock.Object });

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.SetupGet(s => s.ServiceProvider).Returns(scopedProviderMock.Object);
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);
        var publisher = new DomainEventPublisher(scopeFactoryMock.Object);

        // Act
        await publisher.PublishAsync(events);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(event1), Times.Once);
        handlerMock.Verify(h => h.HandleAsync(event2), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_MultipleEvents_ShouldCompleteSuccessfully_WhenEventsCollectionIsEmpty()
    {
        // Arrange
        var events = new List<IDomainEvent>();

        // Act & Assert
        var act = async () => await _publisher.PublishAsync(events);
        await act.Should().NotThrowAsync();
    }

    public sealed class TestDomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public string Message { get; }

        public TestDomainEvent(string message)
        {
            Message = message;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
