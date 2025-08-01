using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Elements.Data.EntityFramework;

namespace Modules.Elements.Data.EntityFramework.MigrationService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _services;
    private readonly IHostEnvironment _environment;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Worker(ILogger<Worker> logger, IServiceProvider services, IHostEnvironment environment, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _services = services;
        _environment = environment;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("starting migration... [EnvironmentName='{EnvironmentName}']", _environment.EnvironmentName);

        try
        {
            using var scope = _services.CreateScope();
            var elementsContext = scope.ServiceProvider.GetRequiredService<ElementsContext>();

            await elementsContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                _logger.LogInformation("migrating database...");
                await elementsContext.Database.MigrateAsync(stoppingToken);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an error occurred while migrating the database: {ErrorMessage}", ex.Message);
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }
}
