namespace Starlights.Integration.Extensions;

public static class IntegrationHostBuilderExtensions
{
    /// <summary>
    /// Configures the integration host to use a console activity processor for OpenTelemetry tracing.
    /// </summary>
    public static IntegrationHostBuilder WithConsoleActivityProcessor(this IntegrationHostBuilder builder)
    {
        builder.ConfigureOptions(o => o.UseConsoleActivityProcessor = true);
        return builder;
    }
}
