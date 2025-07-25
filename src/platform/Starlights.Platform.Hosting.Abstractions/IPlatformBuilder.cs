using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Hosting.Abstractions
{
    public interface IPlatformBuilder
    {
        IServiceCollection Services { get; }
    }
}
