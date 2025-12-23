namespace Starlights.Integration.Extensions;

public static class TestContextExtensions
{
    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Gets the cancellation token associated with the current test context.
        /// </summary>
        /// <remarks>
        /// Use this cancellation token which reflects the lifecycle of the current test, 
        /// allowing for cooperative cancellation of asynchronous operations during test execution.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
        public CancellationToken CancellationToken => host.GetTestContext().CancellationToken;

        /// <summary>
        /// Retrieves the current test context associated with the specified integration host.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
        public TestContext GetTestContext()
        {
            return host.Properties["TestContext"] as TestContext ?? throw new InvalidOperationException("TestContext not found in properties.");
        }

        /// <summary>
        /// Writes the specified message to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        public void WriteLine(string message)
        {
            host.GetTestContext().WriteLine(message);
        }

        /// <summary>
        /// Writes the specified message with an indentation to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        public void WriteIndentedLine(string message, string? indentation = "    ")
        {
            host.WriteLine($"{indentation}{message}");
        }
    }
}