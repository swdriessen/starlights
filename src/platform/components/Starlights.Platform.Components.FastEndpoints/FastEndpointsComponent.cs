using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.FastEndpoints;

/// <summary>
/// Registers the FastEndpoints component with the Starlights Platform.
/// This component should be registered near the end of the registration order to ensure other components are registered first.
/// </summary>
public sealed class FastEndpointsComponent : IPlatformServicesExtension, IPlatformApplicationExtension
{
    int IPlatformServicesExtension.RegistrationOrder => 9000;

    int IPlatformApplicationExtension.RegistrationOrder => 9000;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();
    }

    public void UseExtension(IHost host)
    {
        if (host is not IApplicationBuilder app)
        {
            throw new InvalidOperationException("Host must be an IApplicationBuilder to use FastEndpoints.");
        }

        app.UseFastEndpoints(options => options.Endpoints.RoutePrefix = "api");
    }
}
