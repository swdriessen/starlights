using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;

namespace Starlights.Platform.Components.Data.EntityFramework;

public static class ComponentRegistrationExtensions
{
    /// <summary>
    /// Registers a repository with its associated context factory.
    /// </summary>
    public static IHost UseRepositoryWithContext<TRepository, TContextFactory>(
        this IHost host)
        where TRepository : class, IRepository
        where TContextFactory : class, IPersistenceContextFactory
    {
        var registry = host.Services.GetRequiredService<IContextFactoryRegistry>();
        var factory = host.Services.GetRequiredService<TContextFactory>();
        registry.RegisterContextFactory<TRepository>(factory);

        return host;
    }
}
