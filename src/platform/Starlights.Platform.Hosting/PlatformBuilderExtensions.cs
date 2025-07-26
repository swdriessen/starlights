using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Hosting.Abstractions;

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

        if (builder.Options.IsModuleDiscoveryEnabled)
        {
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

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
                builder.Services.AddSingleton(moduleType, module);
                modules.Add(module);
            }
            else
            {
                // TODO: create custom exceptions for platform exceptions
                throw new InvalidOperationException($"Module type '{moduleType.FullName}' does not implement '{nameof(IPlatformModule)}' or cannot be instantiated.");
            }
        }

        // configure services for modules
        foreach (var module in modules)
        {
            // TODO: provide configuration through method or constructor
            module.ConfigureServices(builder.Services);
        }

        return builder;
    }
}