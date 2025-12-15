
using AwesomeAssertions;
using Starlights.Platform.Domain;
using Starlights.Platform.Eventing;

namespace Starlights.Platform.Tests;

[TestClass]
public sealed class DomainTests
{
    [TestMethod]
    public void EntityBase_ShouldSetId_WhenCreated()
    {
        // Arrange
        const int expectedId = 123;

        // Act
        var entity = new TestEntity(expectedId);

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [TestMethod]
    public void AggregateRoot_ShouldInitializeWithEmptyDomainEvents()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot(Guid.CreateVersion7());

        // Assert
        aggregate.DomainEvents.Should().NotBeNull();
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [TestMethod]
    public void AggregateRoot_ShouldAddDomainEvent_WhenEventIsRaised()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.CreateVersion7());
        var domainEvent = new TestDomainEvent("Test message");

        // Act
        aggregate.RaiseTestEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(1);
        aggregate.DomainEvents.First().Should().BeSameAs(domainEvent);
    }

    [TestMethod]
    public void AggregateRoot_ShouldAddMultipleDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.CreateVersion7());
        var event1 = new TestDomainEvent("Event 1");
        var event2 = new TestDomainEvent("Event 2");

        // Act
        aggregate.RaiseTestEvent(event1);
        aggregate.RaiseTestEvent(event2);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(2);
        aggregate.DomainEvents.Should().Contain(event1);
        aggregate.DomainEvents.Should().Contain(event2);
    }

    [TestMethod]
    public void AggregateRoot_ShouldClearDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.CreateVersion7());
        var domainEvent = new TestDomainEvent("Test message");
        aggregate.RaiseTestEvent(domainEvent);

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [TestMethod]
    public void DomainEvent_ShouldSetOccurredOn_WhenCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new TestDomainEvent("Test");
        var afterCreation = DateTime.UtcNow;

        // Assert
        domainEvent.OccurredOn.Should().BeOnOrAfter(beforeCreation);
        domainEvent.OccurredOn.Should().BeOnOrBefore(afterCreation);
    }

    private sealed class TestEntity : EntityBase<int>
    {
        public TestEntity(int id)
            : base(id)
        {
        }
    }

    private sealed class TestAggregateRoot : AggregateRoot<Guid>
    {
        public TestAggregateRoot(Guid id)
            : base(id)
        {
        }

        public void RaiseTestEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private sealed class TestDomainEvent : IDomainEvent
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
