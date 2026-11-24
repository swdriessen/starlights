using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Modules.Identity.Accounts;

/// <summary>Runs local operator account commands without exposing administration over HTTP.</summary>
public static class AccountCommands
{
    /// <summary>Executes a command, reading any password exclusively from an interactive terminal.</summary>
    public static async Task<int> RunAsync(IServiceProvider services, string[] arguments, CancellationToken cancellationToken = default)
    {
        if (arguments is not [var operation, var username]
            || operation is not ("create" or "reset-password" or "disable" or "enable"))
        {
            Console.Error.WriteLine("Usage: accounts <create|reset-password|disable|enable> <username>. Passwords are prompted, never passed as arguments.");
            return 1;
        }

        using var scope = services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountAdministration>();
        try
        {
            switch (operation)
            {
                case "create":
                    var id = await accounts.CreateAsync(username, ReadConfirmedPassword(), cancellationToken);
                    Console.WriteLine($"Created account {id}.");
                    break;
                case "reset-password":
                    await accounts.ResetPasswordAsync(username, ReadConfirmedPassword(), cancellationToken);
                    Console.WriteLine("Password reset; existing sessions revoked.");
                    break;
                default:
                    await accounts.SetDisabledAsync(username, operation == "disable", cancellationToken);
                    Console.WriteLine(operation == "disable" ? "Account disabled; existing sessions revoked." : "Account re-enabled; sign in again.");
                    break;
            }
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static string ReadConfirmedPassword()
    {
        if (Console.IsInputRedirected)
        {
            throw new InvalidOperationException("Password entry requires an interactive terminal; redirected input is not supported.");
        }

        var password = ReadPassword("Password: ");
        var confirmation = ReadPassword("Confirm password: ");
        if (password != confirmation)
        {
            throw new InvalidOperationException("Passwords do not match.");
        }
        return password;
    }

    private static string ReadPassword(string prompt)
    {
        Console.Write(prompt);
        var value = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return value.ToString();
            }
            if (key.Key == ConsoleKey.Backspace && value.Length > 0)
            {
                value.Length--;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                value.Append(key.KeyChar);
            }
        }
    }
}
