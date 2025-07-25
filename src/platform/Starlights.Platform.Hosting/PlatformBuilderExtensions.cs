using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting
{
    public static class PlatformBuilderExtensions
    {
        public static TBuilder AddStarlightsPlatform<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            ArgumentNullException.ThrowIfNull(builder);

            // TODO: add PlatformBuilderOptions to allow customization of the platform services

            // register the platform services
            var platformBuilder = new PlatformBuilder(builder.Services);

            platformBuilder.Build();

            return builder;
        }
    }
}
