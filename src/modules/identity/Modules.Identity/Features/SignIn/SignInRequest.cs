using System.ComponentModel.DataAnnotations;

namespace Starlights.Modules.Identity.Features.SignIn;

/// <summary>Credentials for an operator-provisioned local account.</summary>
public sealed record SignInRequest
{
    /// <summary>Gets the account username.</summary>
    [Required, MaxLength(256)]
    public required string Username { get; init; }

    /// <summary>Gets the account password.</summary>
    [Required, MaxLength(1024)]
    public required string Password { get; init; }
}
