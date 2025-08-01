using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Application;

namespace Starlights.Integration.Tests;

/// <summary>
/// Integration host for running integration tests against the Starlights application.
/// </summary>
public class IntegrationHost
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationHost()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Integration"));
    }

    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    public IServiceScope CreateServiceScope() => _factory.Services.CreateScope();

    /// <summary>
    /// Creates a new instance of the <see cref="IntegrationHostBuilder"/>.
    /// </summary>
    public static IntegrationHostBuilder CreateBuilder()
    {
        return new IntegrationHostBuilder();
    }
}
