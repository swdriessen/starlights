using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

internal static class PlatformBuilderExtensions
{
    /// <summary>
    /// Discovers all platform modules in the current application domain.
    /// </summary>
    internal static (IEnumerable<Assembly>, IEnumerable<Type>) GetDiscoveredModules(this IPlatformBuilder builder)
    {
        var assemblies = new List<Assembly>();
        var types = new List<Type>();

        if (builder.Options.IsDiscoveryEnabled)
        {
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            assemblies.AddRange(builder.Options.AdditionalAssemblies);

            var discoveredTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IPlatformModule).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .Distinct();

            types.AddRange(discoveredTypes);
        }

        return (assemblies, types);
    }

    /// <summary>
    /// Registers all platform modules found in the current application domain.
    /// </summary>
    internal static IPlatformBuilder RegisterPlatformModules(this IPlatformBuilder builder)
    {
        var modules = new List<IPlatformModule>();

        var (_, types) = builder.GetDiscoveredModules();

        foreach (var moduleType in types)
        {
            // checks for empty constructors 
            if (moduleType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException($"Module type '{moduleType.FullName}' must have a parameterless constructor.");
            }

            // check if type is registered
            if (builder.Services.Any(s => s.ServiceType == moduleType))
            {
                // if the type is already registered, skip it
                continue;
            }

            if (Activator.CreateInstance(moduleType) is IPlatformModule module)
            {
                Platform.WriteLine($"register module [name='{moduleType.FullName}']");
                builder.Services.AddSingleton(moduleType, module);
                modules.Add(module);
            }
            else
            {
                // TODO: create custom exceptions for platform exceptions
                throw new InvalidOperationException($"Module type '{moduleType.FullName}' does not implement '{nameof(IPlatformModule)}' or cannot be instantiated.");
            }
        }

        if (builder.Properties["IHostApplicationBuilder"] is not IHostApplicationBuilder hostApplicationBuilder)
        {
            throw new InvalidOperationException("The IHostApplicationBuilder is not available in the platform builder properties.");
        }

        // configure services for modules
        foreach (var module in modules)
        {
            Platform.WriteLine($"configure module [name='{module.GetType().FullName}']");
            module.ConfigureServices(hostApplicationBuilder);
        }

        return builder;
    }

    /// <summary>
    /// Discovers all platform modules in the current application domain.
    /// </summary>
    internal static (IEnumerable<Assembly>, IEnumerable<Type>) GetPlatformServicesExtension(this IPlatformBuilder builder)
    {
        var assemblies = new List<Assembly>();
        var types = new List<Type>();

        if (builder.Options.IsDiscoveryEnabled)
        {
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            assemblies.AddRange(builder.Options.AdditionalAssemblies);

            var discoveredTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IPlatformServicesExtension).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .Distinct();

            types.AddRange(discoveredTypes);
        }

        return (assemblies, types);
    }

    /// <summary>
    /// Invokes all platform extensions found in the current application domain. These are not registered itself in the container.
    /// </summary>
    internal static IPlatformBuilder InvokePlatformServicesExtensions(this IPlatformBuilder builder)
    {
        var extensions = new List<IPlatformServicesExtension>();

        var (_, types) = builder.GetPlatformServicesExtension();

        foreach (var extensionType in types)
        {
            // checks for empty constructors 
            if (extensionType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException($"Extension type '{extensionType.FullName}' must have a parameterless constructor.");
            }

            if (Activator.CreateInstance(extensionType) is IPlatformServicesExtension extension)
            {
                extensions.Add(extension);
            }
            else
            {
                // TODO: create custom exceptions for platform exceptions
                throw new InvalidOperationException($"Extension type '{extensionType.FullName}' does not implement '{nameof(IPlatformServicesExtension)}' or cannot be instantiated.");
            }
        }

        if (builder.Properties["IHostApplicationBuilder"] is not IHostApplicationBuilder hostApplicationBuilder)
        {
            throw new InvalidOperationException("The IHostApplicationBuilder is not available in the platform builder properties.");
        }

        // invoke the extensions
        foreach (var extension in extensions.OrderBy(e => e.RegistrationOrder))
        {
            Platform.WriteLine($"configure svc [order='{extension.RegistrationOrder}', name='{extension.GetType().FullName}']");
            extension.ConfigureServices(hostApplicationBuilder);
        }

        return builder;
    }
}
