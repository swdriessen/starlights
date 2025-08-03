using System.Diagnostics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CommandExtensions
{
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
}