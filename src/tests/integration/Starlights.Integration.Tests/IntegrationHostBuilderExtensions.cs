using Starlights.Integration.Core;

namespace Starlights.Integration;

internal static class IntegrationHostBuilderExtensions
{
    /// <summary>
    /// Adds the specified <see cref="TestContext"/> to the integration host builder for use during test execution.
    /// The <see cref="TestContext"/> is only available after the class constructor has run.
    /// Therefore, this method should be called in the initializer of the test class.
    /// </summary>
    /// <remarks>This method enables test-specific services or configuration to be accessed during integration
    /// testing by attaching the provided <see cref="TestContext"/> to the builder's properties.</remarks>
    public static IntegrationHostBuilder WithTestContext(this IntegrationHostBuilder builder, TestContext testContext)
    {
        builder.Properties["TestContext"] = testContext;

        builder.ConfigureOptions(o => o.DriverAssemblies = [typeof(IntegrationHostBuilderExtensions).Assembly]);

        return builder;
    }


    /// <summary>
    /// Configures the integration host to use a console activity processor for OpenTelemetry tracing.
    /// </summary>
    public static IntegrationHostBuilder WithConsoleActivityProcessor(this IntegrationHostBuilder builder)
    {
        builder.ConfigureOptions(o => o.UseConsoleActivityProcessor = true);
        return builder;
    }
}
