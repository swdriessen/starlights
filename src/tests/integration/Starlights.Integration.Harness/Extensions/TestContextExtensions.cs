using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration.Extensions;

public static class TestContextExtensions
{
    extension(IntegrationTestContext context)
    {

    }


    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Retrieves the current integration test context associated with the specified integration host.
        /// </summary>
        public IntegrationTestContext IntegrationContext => host.Services.GetRequiredService<IntegrationTestContext>();


        /// <summary>
        /// Retrieves the current test context associated with the specified integration host.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
        public TestContext TestContext => host.Properties["TestContext"] as TestContext ?? throw new InvalidOperationException("TestContext not found in properties.");

        /// <summary>
        /// Gets the cancellation token associated with the current test context.
        /// </summary>
        /// <remarks>
        /// Use this cancellation token which reflects the lifecycle of the current test, 
        /// allowing for cooperative cancellation of asynchronous operations during test execution.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
        public CancellationToken CancellationToken => host.IntegrationContext.CancellationToken;

        /// <summary>
        /// Writes the specified message to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        public void WriteLine(string message)
        {
            host.TestContext.WriteLine(message);
        }

        /// <summary>
        /// Writes the specified message with an indentation to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        public void WriteIndentedLine(string message, string? indentation = "    ")
        {
            host.TestContext.WriteLine($"{indentation}{message}");
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