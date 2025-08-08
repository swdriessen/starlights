using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Eventing.EventPublisher;

public static class DomainEventHandlerRegistrationExtensions
{
    /// <summary>
    /// Registers a concrete domain event handler as itself (scoped) and exposes a scope-per-call wrapper as IDomainEventHandler{TEvent}.
    /// </summary>
    public static IServiceCollection AddDomainEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : class, IDomainEvent
        where THandler : class, IDomainEventHandler<TEvent>
    {
        services.AddScoped<THandler>();
        services.AddSingleton<IDomainEventHandler<TEvent>, ScopedDomainEventHandler<TEvent, THandler>>();
        return services;
    }

    /// <summary>
    /// Scans the given assembly for IDomainEventHandler{TEvent} implementations and registers them using the wrapper pattern.
    /// </summary>
    public static IServiceCollection AddDomainEventHandlersFrom(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterface = typeof(IDomainEventHandler<>);

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var implemented = type.GetInterfaces()
                                  .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
                                  .ToArray();

            foreach (var @interface in implemented)
            {
                var eventType = @interface.GetGenericArguments()[0];
                RegisterClosedWrapper(services, eventType, type);
            }
        }

        return services;

        static void RegisterClosedWrapper(IServiceCollection services, Type eventType, Type handlerType)
        {
            // Call: services.AddDomainEventHandler<TEvent, THandler>()
            var method = typeof(DomainEventHandlerRegistrationExtensions)
                .GetMethod(nameof(AddDomainEventHandler))!
                .MakeGenericMethod(eventType, handlerType);

            _ = method.Invoke(null, new object[] { services })!;
        }
    }
}
