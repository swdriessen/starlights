using System.Reflection;

namespace Starlights.Platform.Hosting;

internal static class PlatformHostExtensions
{
    /// <summary>
    /// Discovers all platform components in the current application domain.
    /// </summary>
    internal static (IEnumerable<Assembly>, IEnumerable<Type>) GetPlatformComponents<T>(this IPlatform platform)
    {
        var assemblies = new List<Assembly>();
        var types = new List<Type>();

        if (platform.Options.IsDiscoveryEnabled)
        {
            assemblies.AddRange(GetCandidateAssemblies());
            assemblies.AddRange(platform.Options.AdditionalAssemblies);

            var discoveredTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .Distinct();

            types.AddRange(discoveredTypes);
        }

        return (assemblies, types);
    }

    /// <summary>
    /// Invokes all platform application components found in the current application domain.
    /// </summary>
    internal static IPlatform InvokeApplicationComponents(this IPlatform platform)
    {
        var components = new List<IPlatformApplicationComponent>();

        var (_, types) = platform.GetPlatformComponents<IPlatformApplicationComponent>();

        foreach (var componentType in types)
        {
            // checks for empty constructors 
            if (componentType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException($"Component type '{componentType.FullName}' must have a parameterless constructor.");
            }

            if (Activator.CreateInstance(componentType) is IPlatformApplicationComponent component)
            {
                components.Add(component);
            }
            else
            {
                // TODO: create custom exceptions for platform exceptions
                throw new InvalidOperationException($"Component type '{componentType.FullName}' does not implement '{nameof(IPlatformApplicationComponent)}' or cannot be instantiated.");
            }
        }

        Platform.WriteLine("========== Configure Application Components ==========");

        // invoke the components
        foreach (var component in components.OrderBy(e => e.RegistrationOrder))
        {
            Platform.WriteLine($"configure app [order='{component.RegistrationOrder}', component='{component.GetType().FullName}']");
            component.UseComponent(platform.Host);
        }

        return platform;
    }

    private static IEnumerable<Assembly> GetCandidateAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(IsCandidateAssembly);


        static bool IsCandidateAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
            {
                return false;
            }

            var name = assembly.GetName().Name ?? string.Empty;

            if (string.Equals(name, "DynamicProxyGenAssembly2", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (name.StartsWith("Castle.Proxies", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }

    private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return [];
        }
    }
}