using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Starlights.Platform.Data;

namespace Starlights.Platform.Components.Data.EntityFramework;

public class Persistence : IPersistence
{
    private readonly ILogger<Persistence> _logger;
    private readonly IPersistenceContextFactory _contextFactory;
    private readonly IServiceProvider _serviceProvider;
    private IPersistenceContext? _persistenceContext;
    private bool _disposedValue;

    public Persistence(ILogger<Persistence> logger, IPersistenceContextFactory contextFactory, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _serviceProvider = serviceProvider;
    }

    public T GetRepository<T>() where T : IRepository
    {
        _logger.LogInformation("get repository [type='{RepositoryType}']", typeof(T).Name);

        var repository = _serviceProvider.GetRequiredService<T>(); // TODO: investigate cache in case of multiple calls

        _persistenceContext ??= _contextFactory.CreateContext();

        repository.SetPersistenceContext(_persistenceContext);

        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        if (_persistenceContext is not DbContext context)
        {
            throw new ArgumentException("The provided context is not a valid DbContext.", nameof(_persistenceContext));
        }


        _logger.LogInformation("saving...");
        var result = await context.SaveChangesAsync();
        _logger.LogInformation("...saved successfully [rows='{Rows}', context='{RepositoryType}']", result, context.GetType().Name);

        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_persistenceContext is DbContext context)
                {
                    context.Dispose();
                }
            }

            _persistenceContext = null;
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
