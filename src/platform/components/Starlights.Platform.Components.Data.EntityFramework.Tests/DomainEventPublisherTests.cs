using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Starlights.Platform.Eventing;

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
        _publisher = new DomainEventPublisher(_serviceProvider);
    }

    [TestMethod]
    public void Constructor_ShouldCreateInstance_WhenServiceProviderIsProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var publisher = new DomainEventPublisher(serviceProvider);

        // Assert
        publisher.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PublishAsync_SingleEvent_ShouldInvokeHandler_WhenHandlerExists()
    {
        // Arrange
        var domainEvent = new TestDomainEvent("Test message");
        var handlerMock = new Mock<IDomainEventHandler<TestDomainEvent>>();

        var services = new ServiceCollection();
        services.AddSingleton(handlerMock.Object);
        var serviceProvider = services.BuildServiceProvider();
        var publisher = new DomainEventPublisher(serviceProvider);

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

        var services = new ServiceCollection();
        services.AddSingleton(handlerMock1.Object);
        services.AddSingleton(handlerMock2.Object);
        var serviceProvider = services.BuildServiceProvider();
        var publisher = new DomainEventPublisher(serviceProvider);

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

        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var publisher = new DomainEventPublisher(serviceProvider);

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

        var services = new ServiceCollection();
        services.AddSingleton(handlerMock.Object);
        var serviceProvider = services.BuildServiceProvider();
        var publisher = new DomainEventPublisher(serviceProvider);

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
