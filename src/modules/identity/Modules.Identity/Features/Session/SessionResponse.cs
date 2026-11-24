namespace Starlights.Modules.Identity.Features.Session;

/// <summary>Identifies the currently authenticated account without exposing credentials.</summary>
/// <param name="UserId">The stable account identifier used for resource ownership.</param>
/// <param name="Username">The account's current username.</param>
public sealed record SessionResponse(Guid UserId, string Username);
