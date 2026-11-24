namespace Starlights.Platform.Hosting;

/// <summary>Provides the trusted identity of the user making the current request.</summary>
public interface ICurrentUser
{
    /// <summary>Gets the stable user identifier, or null outside an authenticated request.</summary>
    Guid? UserId { get; }
}
