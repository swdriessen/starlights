using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public static class HostingExtensions
{
    public static TBuilder AddStarlightsPlatform<TBuilder>(this TBuilder builder, Action<PlatformBuilderOptions>? optionsAction = null)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new PlatformBuilderOptions();
        optionsAction?.Invoke(options);

        // register the platform services
        var platformBuilder = new PlatformBuilder(builder.Services, options);
        platformBuilder.Build();

        return builder;
    }
}
