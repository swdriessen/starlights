using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.Data.EntityFramework;

/// <summary>
/// The component for the Starlights Platform that configures the Entity Framework services.
/// </summary>
public class PlatformEntityFrameworkComponent : IPlatformServiceComponent
{
    public int RegistrationOrder => 500;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPersistence, Persistence>();
        builder.Services.AddSingleton<IContextFactoryRegistry, ContextFactoryRegistry>();
    }
}

public static class PlatformEntityFrameworkComponentExtensions
{
    /// <summary>
    /// Registers a repository with its associated context factory.
    /// </summary>
    ////public static IServiceCollection AddRepositoryWithContext<TRepository, TContextFactory>(
    ////    this IServiceCollection services)
    ////    where TRepository : class, IRepository
    ////    where TContextFactory : class, IPersistenceContextFactory
    ////{
    ////    //services.AddScoped<TRepository>();
    ////    services.AddScoped<TContextFactory>();

    ////    services.AddSingleton(provider =>
    ////    {
    ////        var registry = provider.GetRequiredService<IContextFactoryRegistry>();
    ////        var factory = provider.GetRequiredService<TContextFactory>();
    ////        registry.RegisterContextFactory<TRepository>(factory);
    ////        return registry;
    ////    });

    ////    return services;
    ////}

    /// <summary>
    /// Registers a repository with its associated context factory.
    /// </summary>
    public static IHost UseRepositoryWithContext<TRepository, TContextFactory>(
        this IHost host)
        where TRepository : class, IRepository
        where TContextFactory : class, IPersistenceContextFactory
    {
        //services.AddScoped<TRepository>();

        var registry = host.Services.GetRequiredService<IContextFactoryRegistry>();
        var factory = host.Services.GetRequiredService<TContextFactory>();
        registry.RegisterContextFactory<TRepository>(factory);

        return host;
    }

}
