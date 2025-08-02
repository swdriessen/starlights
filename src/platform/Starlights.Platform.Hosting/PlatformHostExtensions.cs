using System.Reflection;

namespace Starlights.Platform.Hosting;

internal static class PlatformHostExtensions
{
    /// <summary>
    /// Discovers all platform modules in the current application domain.
    /// </summary>
    internal static (IEnumerable<Assembly>, IEnumerable<Type>) GetPlatformExtensions<T>(this IPlatform platform)
    {
        var assemblies = new List<Assembly>();
        var types = new List<Type>();

        if (platform.Options.IsDiscoveryEnabled)
        {
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            assemblies.AddRange(platform.Options.AdditionalAssemblies);

            var discoveredTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .Distinct();

            types.AddRange(discoveredTypes);
        }

        return (assemblies, types);
    }

    /// <summary>
    /// Invokes all platform extensions found in the current application domain.
    /// </summary>
    internal static IPlatform InvokeApplicationExtensions(this IPlatform platform)
    {
        var extensions = new List<IPlatformApplicationExtension>();

        var (_, types) = platform.GetPlatformExtensions<IPlatformApplicationExtension>();

        foreach (var extensionType in types)
        {
            // checks for empty constructors 
            if (extensionType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException($"Extension type '{extensionType.FullName}' must have a parameterless constructor.");
            }

            if (Activator.CreateInstance(extensionType) is IPlatformApplicationExtension extension)
            {
                extensions.Add(extension);
            }
            else
            {
                // TODO: create custom exceptions for platform exceptions
                throw new InvalidOperationException($"Extension type '{extensionType.FullName}' does not implement '{nameof(IPlatformApplicationExtension)}' or cannot be instantiated.");
            }
        }

        // invoke the extensions
        foreach (var extension in extensions.OrderBy(e => e.RegistrationOrder))
        {
            Platform.WriteLine($"configure app [order='{extension.RegistrationOrder}', name='{extension.GetType().FullName}']");
            extension.UseExtension(platform.Host);
        }

        return platform;
    }
}