namespace Starlights.Integration.Core.Extensions;

public static class TestContextExtensions
{
    /// <summary>
    /// Retrieves the cancellation token associated with the current test context from the integration host.
    /// </summary>
    /// <remarks>
    /// Use this method to obtain a cancellation token that reflects the lifecycle of the current
    /// test, allowing for cooperative cancellation of asynchronous operations during test execution.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
    public static CancellationToken GetTestCancellationToken(this IIntegrationHost host)
    {
        var context = host.GetTestContext();
        return context.CancellationToken;
    }

    /// <summary>
    /// Retrieves the current test context associated with the specified integration host.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the integration host does not contain a test context in its properties.</exception>
    public static TestContext GetTestContext(this IIntegrationHost host)
    {
        return host.Properties["TestContext"] as TestContext ?? throw new InvalidOperationException("TestContext not found in properties.");
    }

    /// <summary>
    /// Writes the specified message to the test output associated with the integration host.
    /// </summary>
    /// <param name="message">The message to write to the test output.</param>
    public static void WriteLine(this IIntegrationHost host, string? message)
    {
        host.GetTestContext().WriteLine(message);
    }
}