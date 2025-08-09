using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Eventing.EventPublisher;

/// <summary>
/// Wrapper that creates a DI scope per HandleAsync call, resolves the concrete handler, and delegates the call.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
/// <typeparam name="THandler">The concrete handler type.</typeparam>
public sealed class ScopedDomainEventHandler<TEvent, THandler> : IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
    where THandler : class, IDomainEventHandler<TEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedDomainEventHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc />
    public async Task HandleAsync(TEvent domainEvent)
    {
        using var scope = _scopeFactory.CreateScope();
        var inner = scope.ServiceProvider.GetRequiredService<THandler>();
        await inner.HandleAsync(domainEvent).ConfigureAwait(false);
    }
}
