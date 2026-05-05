using System.Diagnostics;
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
            if (testTimeout != default)
            {
                builder.ConfigureOptions(options => options.TestTimeout = testTimeout);
            }

            builder.ConfigureServices((services) =>
            {
                services.AddSingleton(serviceProvider =>
                {
                    var options = serviceProvider.GetRequiredService<IntegrationHostOptions>();

                    // if a debugger is attached, we want to use an "infinite" timeout to avoid timeouts while stepping through code
                    var timeout = Debugger.IsAttached ? TimeSpan.FromDays(1) : options.TestTimeout;

                    return new IntegrationTestContext(testContext, timeout);
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
        /// Gets a cancellation token that is linked to the test context's cancellation token, allowing for cooperative cancellation of asynchronous operations during test execution.
        /// </summary>
        public CancellationToken CancellationToken => host.IntegrationContext.CancellationToken;

        /// <summary>
        /// Stores a value of the specified type in the integration host's properties dictionary, using either the provided key or the full name of the type as the key. This allows for sharing data across different parts of the integration test by storing it with a specific key or type and retrieving it later using the same key or type.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Set<T>(T value, string? key = null) where T : notnull
        {
            ArgumentNullException.ThrowIfNull(value);

            var actualKey = key ?? typeof(T).FullName
                ?? throw new InvalidOperationException("Type FullName is null. Cannot use null as a dictionary key.");

            host.Properties[actualKey] = value;
        }

        /// <summary>
        /// Retrieves a value of the specified type from the integration host's properties dictionary, using either the provided key or the full name of the type as the key. If the key is not found in the dictionary, an exception is thrown. This allows for sharing data across different parts of the integration test by storing it with a specific key or type and retrieving it later using the same key or type.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
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