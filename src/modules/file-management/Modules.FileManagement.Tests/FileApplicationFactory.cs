using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starlights.Application;
using Starlights.Modules.FileManagement.Data;
using Starlights.Modules.FileManagement.Storage;

namespace Starlights.Modules.FileManagement.Tests;

internal sealed class FileApplicationFactory(string databasePath) : WebApplicationFactory<Program>
{
    public TestFileContentStore Content { get; } = new();
    public FailingFileSaveInterceptor PersistenceFailure { get; } = new();
    public FailingFileDeleteInterceptor DeletionFailure { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<FileManagementContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<FileManagementContext>>();
            services.AddDbContext<FileManagementContext>(options => options.UseSqlite($"Data Source={databasePath};Pooling=False").AddInterceptors(PersistenceFailure, DeletionFailure));
            services.RemoveAll<IFileContentStore>();
            services.AddSingleton<IFileContentStore>(Content);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Path.GetDirectoryName(databasePath)!, "keys")))
                .UseEphemeralDataProtectionProvider();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "FileTests";
                options.DefaultChallengeScheme = "FileTests";
            }).AddScheme<AuthenticationSchemeOptions, FileTestAuthenticationHandler>("FileTests", _ => { });
        });
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<FileManagementContext>().Database.EnsureCreatedAsync(cancellationToken);
    }

    public HttpClient Browser(Guid? owner = null)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        if (owner.HasValue)
            client.DefaultRequestHeaders.Add("Test-Owner", owner.Value.ToString());
        return client;
    }
}
