namespace Starlights.Integration;

public interface IIntegrationHost
{
    /// <summary>
    /// Gets a dictionary of properties associated with this host.
    /// </summary>
    Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the services available in this host.
    /// </summary>
    IServiceProvider Services { get; }

    /// <summary>
    /// Creates a new HTTP client for making requests to the application.
    /// </summary>
    HttpClient CreateClient();
}

public sealed class IntegrationTestContext : IDisposable
{
    private readonly CancellationTokenSource _cancellationSource;
    private readonly TestContext _testContext;
    private bool _disposedValue;

    public IntegrationTestContext(TestContext testContext, TimeSpan testTimeout)
    {
        _testContext = testContext;
        _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_testContext.CancellationToken);
        _cancellationSource.CancelAfter(testTimeout);
    }

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