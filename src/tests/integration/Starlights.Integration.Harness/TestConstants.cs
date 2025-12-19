namespace Starlights.Integration;

public sealed class TestConstants
{
#if DEBUG
    /// <summary>
    /// Specifies the timeout value used in debug builds.
    /// </summary>
    /// <remarks>This constant is set to <see cref="int.MaxValue"/>, effectively disabling timeouts during
    /// debugging. It is only available when the DEBUG symbol is defined.</remarks>
    public const int Timeout = int.MaxValue;
#else
    /// <summary>
    /// Specifies the timeout value, in milliseconds, used for operations in release builds.
    /// </summary>
    public const int Timeout = 5_000; // 5 seconds in release mode, i.e. in CI/CD
#endif
}
