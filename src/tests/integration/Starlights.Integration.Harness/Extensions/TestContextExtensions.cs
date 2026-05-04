using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration.Extensions;

public static class TestContextExtensions
{
    extension(IntegrationHostBuilder builder)
    {
        /// <summary>
        /// Configures the integration host builder to include the specified test context, allowing for access to test-specific information and functionality during integration testing.
        /// </summary>
        /// <param name="testContext">The test context to include in the integration host builder.</param>
        /// <param name="testTimeout">The timeout duration for the test context.</param>
        /// <returns>The updated integration host builder.</returns>
        public IntegrationHostBuilder WithTestContext(TestContext testContext, TimeSpan testTimeout = default)
        {
            builder.ConfigureOptions(o => o.TestTimeout = testTimeout == default ? TimeSpan.FromSeconds(10) : testTimeout);
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton(serviceProvider =>
                {
                    var configuredOptions = serviceProvider.GetRequiredService<IntegrationHostOptions>();
                    return new IntegrationTestContext(testContext, configuredOptions.TestTimeout);
                });
            });

            return builder;
        }
    }

    extension(IntegrationTestContext context)
    {
        /// <summary>
        /// Writes the specified message to the test output associated with the integration test context, allowing for logging and debugging information to be captured during integration test execution.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        public void WriteLine(string message)
        {
            context.TestContext.WriteLine(message);
        }
    }

    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Retrieves the current integration test context associated with the specified integration host.
        /// </summary>
        public IntegrationTestContext IntegrationContext => host.Services.GetRequiredService<IntegrationTestContext>();

        /// <summary>
        /// Gets the cancellation token associated with the current test context.
        /// </summary>
        /// <remarks>
        /// Use this cancellation token which reflects the lifecycle of the current test, 
        /// allowing for cooperative cancellation of asynchronous operations during test execution.
        /// </remarks>
        [Obsolete("Use the CancellationToken property on the IntegrationTestContext instead.")]
        public CancellationToken CancellationToken => host.IntegrationContext.CancellationToken;

        /// <summary>
        /// Writes the specified message to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        [Obsolete("Use the WriteLine method on the IntegrationTestContext instead.")]
        public void WriteLine(string message)
        {
            host.IntegrationContext.TestContext.WriteLine(message);
        }

        /// <summary>
        /// Sets a value in the scenario context.
        /// </summary>
        public void Set<T>(T value, string? key = null) where T : notnull
        {
            ArgumentNullException.ThrowIfNull(value);

            var actualKey = key ?? typeof(T).FullName
                ?? throw new InvalidOperationException("Type FullName is null. Cannot use null as a dictionary key.");

            host.Properties[actualKey] = value;
        }

        /// <summary>
        /// Gets a value from the scenario context.
        /// </summary>
        public T Get<T>(string? key = null)
        {
            var actualKey = key ?? typeof(T).FullName
                ?? throw new InvalidOperationException("Type FullName is null. Cannot use null as a dictionary key.");

            return host.Properties.TryGetValue(actualKey, out var value)
                ? (T)value
                : throw new KeyNotFoundException($"Key '{actualKey}' not found in scenario context.");
        }
    }
}