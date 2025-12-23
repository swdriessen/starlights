namespace Starlights.Integration;

/// <summary>
/// Provides a base class for integration tests that require access to the current test context.
/// </summary>
/// <remarks>This class is intended to be inherited by integration test classes to enable access to the test
/// framework's context and utilities. The TestContext property allows tests to interact with test output, logging, and
/// other test-related services.</remarks>
public abstract class IntegrationTestBase
{
    public TestContext TestContext { get; set; } = default!;
}
