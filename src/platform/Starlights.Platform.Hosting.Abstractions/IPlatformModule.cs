using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Hosting.Abstractions;

public interface IPlatformModule
{
    void ConfigureServices(IServiceCollection services);
}
