using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Domain;

namespace Modules.Characters.Data.EntityFramework.MigrationService;

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
        using var activity = CharactersInstrumentation.StartActivity("ExecuteAsync");

        _logger.LogInformation("starting migration... [EnvironmentName='{EnvironmentName}']", _environment.EnvironmentName);

        try
        {
            using var scope = _services.CreateScope();
            var elementsContext = scope.ServiceProvider.GetRequiredService<CharactersContext>();

            await elementsContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var _ = CharactersInstrumentation.StartActivity("Migrate CharactersContext");
                _logger.LogInformation("migrating database...");
                await elementsContext.Database.MigrateAsync(stoppingToken);
            });
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            _logger.LogError(ex, "an error occurred while migrating the database: {ErrorMessage}", ex.Message);
            throw;
        }
        finally
        {
            _logger.LogInformation("stopping migration worker... [EnvironmentName='{EnvironmentName}']", _environment.EnvironmentName);
        }

        _hostApplicationLifetime.StopApplication();
    }
}
