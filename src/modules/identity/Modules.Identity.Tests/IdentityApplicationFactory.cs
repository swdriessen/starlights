using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starlights.Application;
using Starlights.Modules.Identity.Data;

namespace Starlights.Modules.Identity.Tests;

internal sealed class IdentityApplicationFactory(string databasePath, TestClock clock) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<IdentityContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<IdentityContext>>();
            services.AddDbContext<IdentityContext>(options => options.UseSqlite($"Data Source={databasePath};Pooling=False"));
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(clock);
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.EnsureCreatedAsync();
    }

    public HttpClient CreateBrowser()
        => CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
}
