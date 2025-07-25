using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Hosting;

public sealed class PlatformBuilder : IPlatformBuilder
{
    public PlatformBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public void Build()
    {
        RegisterPlatformModules();
    }

    /// <summary>
    /// Registers all platform modules found in the current AppDomain.
    /// </summary>
    private void RegisterPlatformModules()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = GetTypesAssignableFromInterface<IPlatformModule>(assemblies);

        var modules = new List<IPlatformModule>();

        // register modules, add the module instance to the service collection as a singleton
        foreach (var moduleType in types)
        {
            // TODO: add checks for empty constructors or specific constructor signatures if needed <TBD>

            if (Activator.CreateInstance(moduleType) is IPlatformModule module)
            {
                modules.Add(module);
                Services.AddSingleton(moduleType, module);
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

            module.ConfigureServices(Services);
        }
    }

    private static Type[] GetTypesAssignableFromInterface<T>(IEnumerable<Assembly> assemblies) where T : IPlatformModule
    {
        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .Distinct();

        return [.. types];
    }
}
