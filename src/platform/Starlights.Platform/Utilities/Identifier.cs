namespace Starlights.Platform.Utilities;

public static class Identifier
{
    /// <summary>
    /// Generates a new unique identifier (GUID) using the version 7 format.
    /// </summary>
    public static Guid NewGuid() => Guid.CreateVersion7();
}
