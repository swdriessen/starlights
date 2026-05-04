namespace Starlights.Integration;

public sealed class IntegrationTestContext : IDisposable
{
    private readonly CancellationTokenSource _cancellationSource;
    private bool _disposedValue;

    public IntegrationTestContext(TestContext testContext, TimeSpan testTimeout)
    {
        TestContext = testContext;

        _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        _cancellationSource.CancelAfter(testTimeout);
        _cancellationSource.Token.Register(() => TestContext.WriteLine($"Test execution cancelled after exceeding the timeout of {testTimeout.TotalSeconds} seconds."));
    }

    /// <summary>
    /// Gets the test context associated with this integration test context, allowing for access to test-specific information and functionality during integration testing.
    /// </summary>
    internal TestContext TestContext { get; }

    /// <summary>
    /// Gets a cancellation token that is linked to the test context's cancellation token, allowing for cooperative cancellation of asynchronous operations during test execution.
    /// </summary>
    public CancellationToken CancellationToken => _cancellationSource.Token;

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cancellationSource.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}