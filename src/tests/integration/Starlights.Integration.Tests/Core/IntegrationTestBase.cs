namespace Starlights.Integration.Core;

public abstract class IntegrationTestBase
{
    public TestContext TestContext { get; set; } = default!;

    public CancellationToken TestCancellationToken => TestContext.CancellationToken;
}
