using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting.Abstractions;

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
}
