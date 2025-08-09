# Domain Event Handling (scope-per-handler wrapper)

This component provides a simple pattern to ensure each domain event handler runs in its own DI scope (so scoped dependencies like DbContext are isolated), while keeping the publisher lean.

## What you get

- A wrapper that creates a scope per handler invocation
- DI helpers to register handlers via the wrapper (single or assembly scan)
- A publisher that relies on wrappers (no per-handler scope logic)

## Wrapper

```csharp
// Starlights.Platform.Components.Data.EntityFramework
// ScopedDomainEventHandler<TEvent, THandler>
using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Eventing;

public sealed class ScopedDomainEventHandler<TEvent, THandler> : IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
    where THandler : class, IDomainEventHandler<TEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedDomainEventHandler(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task HandleAsync(TEvent domainEvent)
    {
        using var scope = _scopeFactory.CreateScope();
        var inner = scope.ServiceProvider.GetRequiredService<THandler>();
        await inner.HandleAsync(domainEvent).ConfigureAwait(false);
    }
}
```

## Registration

Register a single handler:

```csharp
// Registers the concrete handler as Scoped and exposes the wrapper as IDomainEventHandler<TEvent> (Singleton)
services.AddDomainEventHandler<MyEvent, MyEventHandler>();
```

Scan an assembly for all handlers:

```csharp
services.AddDomainEventHandlersFrom(typeof(MyEventHandler).Assembly);
```

Notes:

- Concrete handlers can be `Scoped` and depend on `DbContext` safely.
- Wrappers are `Singleton`; they open a scope per `HandleAsync` call.

## Example handler

```csharp
using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Eventing;

public sealed class MyEvent : IDomainEvent { /* ... */ }

public sealed class MyEventHandler(MyDbContext db) : IDomainEventHandler<MyEvent>
{
    public async Task HandleAsync(MyEvent ev)
    {
        // use db safely; each invocation has its own scope
        await db.SaveChangesAsync();
    }
}
```

## Publisher behavior

`DomainEventPublisher` discovers `IEnumerable<IDomainEventHandler<TEvent>>` (the wrappers) and invokes `HandleAsync` on each. Each wrapper takes care of creating a scope and delegating to the concrete handler.

```csharp
await publisher.PublishAsync(new MyEvent());
```

- Multiple handlers for a single event are supported.
- Publishing multiple events runs handlers concurrently per event.

## Why this pattern?

- Keeps scope management in one place (the wrapper) instead of the publisher.
- Ensures scoped dependencies are isolated across handlers and invocations.
- Simple registration and discoverability via `IEnumerable<IDomainEventHandler<T>>`.

## Testing tips

- Mock `IDomainEventHandler<T>` if you want to test the publisher in isolation.
- For integration tests, register real handlers and verify side effects (DB changes, messages, etc.).
