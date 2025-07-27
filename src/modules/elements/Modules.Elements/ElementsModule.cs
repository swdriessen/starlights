using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Elements.Application.Services;
using Starlights.Modules.Elements.Integration.Abstractions;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Modules.Elements;

public sealed class ElementsModule : IPlatformModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IElementsModuleGateway, ElementsModuleGateway>();
    }
}
