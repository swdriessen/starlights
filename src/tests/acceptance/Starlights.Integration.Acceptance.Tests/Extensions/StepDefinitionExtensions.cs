using Starlights.Integration.Extensions;

namespace Starlights.Integration.Acceptance.Tests.Extensions;

internal static class StepDefinitionExtensions
{
    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Writes the specified message to the test output associated with the integration host.
        /// </summary>
        /// <param name="message">The message to write to the test output.</param>
        internal void WriteStepNotImplemented(string? message = null)
        {
            host.TestContext.WriteLine($"-> warn: this step is skipped, because it is not implemented{(string.IsNullOrWhiteSpace(message) ? string.Empty : $": {message}")}");
        }
    }
}