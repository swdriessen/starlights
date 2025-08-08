using System.Diagnostics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CommandExtensions
{
    /// <summary>
    /// Adds a scalar URL to the project resource.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithScalarUrl(this IResourceBuilder<ProjectResource> resource)
    {
        var httpsEndpoint = resource.GetEndpoint("https");
        resource.WithUrl($"{httpsEndpoint}/scalar/v1", "Scalar UI");
        return resource;
    }

    /// <summary>
    /// Adds a command to the project resource that opens the Scalar UI in a web browser.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithScalarCommand(this IResourceBuilder<ProjectResource> resource)
    {
        var options = new CommandOptions()
        {
            IconName = "CloudArrowRight",
            IconVariant = IconVariant.Regular,
            IsHighlighted = true
        };

        return resource.WithCommand("browse-scalar", "Scalar UI", _ =>
        {
            try
            {
                var endpoint = resource.GetEndpoint("https");
                var url = $"{endpoint.Url}/scalar/v1";

                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true, });

                return Task.FromResult(new ExecuteCommandResult() { Success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex: {ex.Message}");
                return Task.FromResult(new ExecuteCommandResult() { Success = false });
            }
        }, commandOptions: options);
    }

    /// <summary>
    /// Adds a command to the project resource that initializes the database.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithInitializeDatabaseCommand(this IResourceBuilder<ProjectResource> resource)
    {
        return resource.WithHttpCommand(path: "/api/elements/initialize",
            displayName: "Initialize Database",
            commandOptions: new HttpCommandOptions()
            {
                Description = "Initialize the elements in the database.",
                Method = HttpMethod.Get,
                PrepareRequest = (_) => Task.CompletedTask,
                EndpointSelector = () => resource.GetEndpoint("https"),
                IconName = "DatabaseLightning",
                IconVariant = IconVariant.Regular,
                IsHighlighted = false
            });
    }
}