using System.Diagnostics;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130

internal static class DemoAccountCommandExtensions
{
    private const string SuccessMarker = "DEMO_ACCOUNT_COMMAND_SUCCEEDED";
    private const string ResultMarker = "DEMO_ACCOUNT_COMMAND_RESULT_END";

    internal static async Task<int> RunTerminalAsync()
    {
        var projectPath = new Projects.Starlights_Application().ProjectPath;
#if DEBUG
        const string buildConfiguration = "Debug";
#else
        const string buildConfiguration = "Release";
#endif
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)
                ?? throw new InvalidOperationException("The application project directory is unavailable."),
            ArgumentList = { "run", "--no-build", "--no-launch-profile", "--configuration", buildConfiguration, "--project", projectPath, "--", "accounts", "create", "demo" }
        };
        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The demo account command could not be started.");
        await process.WaitForExitAsync();

        if (process.ExitCode == 0)
        {
            Console.WriteLine(SuccessMarker);
        }
        Console.WriteLine(ResultMarker);
        // Keep the terminal alive until the dashboard command observes the result.
        Console.ReadKey(intercept: true);
        return process.ExitCode;
    }

    internal static IResourceBuilder<ProjectResource> WithDemoAccountCommand(
        this IResourceBuilder<ProjectResource> resource,
        IConfiguration configuration,
        IResourceWithConnectionString database)
    {
        return resource.WithCommand("create-demo-account", "Create demo account", async context =>
        {
            var password = configuration["DemoAccount:Password"];
            if (string.IsNullOrWhiteSpace(password) || password.Any(char.IsControl))
            {
                return CommandResults.Failure("Set a non-empty DemoAccount:Password without control characters in AppHost user secrets and restart the AppHost. See the README.");
            }

            var connectionString = await database.GetConnectionStringAsync(context.CancellationToken);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return CommandResults.Failure("The Identity database connection string is unavailable.");
            }

#pragma warning disable ASPIRETERMINAL001 // The existing account command requires interactive terminal input.
            var options = new TerminalLaunchOptions
            {
                Title = "Create demo account",
                Executable = "dotnet",
                Arguments = { typeof(DemoAccountCommandExtensions).Assembly.Location, "--demo-account-terminal" },
                Placement = TerminalPlacement.None
            };
            options.EnvironmentVariables["ConnectionStrings__starlights-db"] = connectionString;

            var terminals = context.Services.GetRequiredService<TerminalService>();
            await using var terminal = terminals.CreateTerminal(options);
            try
            {
                var token = context.CancellationToken;
                await terminal.WaitForTextAsync("Password: ", TimeSpan.FromSeconds(60), token);
                await terminal.SendTextAsync(password, token);
                await terminal.SendKeyAsync(AspireTerminalKey.Enter, token);
                await terminal.WaitForTextAsync("Confirm password: ", TimeSpan.FromSeconds(30), token);
                await terminal.SendTextAsync(password, token);
                await terminal.SendKeyAsync(AspireTerminalKey.Enter, token);
                await terminal.WaitForTextAsync(ResultMarker, TimeSpan.FromSeconds(60), token);
                var screen = terminal.GetScreenText();
                if (screen.Contains(SuccessMarker, StringComparison.Ordinal))
                {
                    return CommandResults.Success("Demo account created.");
                }
                if (screen.Contains("Username 'demo' is already taken.", StringComparison.Ordinal))
                {
                    return CommandResults.Failure("The demo account already exists; it was not changed.");
                }
                if (screen.Contains("Passwords must ", StringComparison.Ordinal))
                {
                    return CommandResults.Failure("The configured demo password does not meet the Identity password policy.");
                }
                return CommandResults.Failure("The account command failed. Check whether demo exists and verify the database; see the README for operator commands.");
            }
            catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
            {
                return CommandResults.Canceled();
            }
            catch (TimeoutException)
            {
                return CommandResults.Failure("Account creation timed out without confirming success. Check whether demo exists before retrying; see the README for operator commands.");
            }
            catch (InvalidOperationException)
            {
                return CommandResults.Failure("The account command stopped without confirming success. Check whether demo exists and verify the password policy and database; see the README for operator commands.");
            }
#pragma warning restore ASPIRETERMINAL001
        }, commandOptions: new CommandOptions
        {
            UpdateState = context => context.ResourceSnapshot.State?.Text == KnownResourceStates.Running
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled
        });
    }
}
