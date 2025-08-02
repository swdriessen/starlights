using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public class Platform : IPlatform
{
    public Platform(IHost host, PlatformHostOptions options)
    {
        Host = host;
        Options = options;
    }

    public IHost Host { get; }

    public PlatformHostOptions Options { get; }

    /// <summary>
    /// Writes a message to the console with a timestamp and platform prefix.
    /// </summary>
    internal static void WriteLine(string message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss:fff} PLT] {message} ");
    }
}
