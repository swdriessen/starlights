namespace Starlights.Modules.Identity.Accounts;

internal interface IAccountAdministration
{
    Task<Guid> CreateAsync(string username, string password, CancellationToken cancellationToken);
    Task ResetPasswordAsync(string username, string password, CancellationToken cancellationToken);
    Task SetDisabledAsync(string username, bool disabled, CancellationToken cancellationToken);
}
