using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Data;
using Starlights.Platform.Domain;

namespace Starlights.Platform.Components.Data.EntityFramework;

public abstract class RepositoryBase<TEntity> : IRepository
    where TEntity : class, IEntity
{
    private DbContext _context = default!;
    private DbSet<TEntity> _entities = default!;

    protected DbContext Context
    {
        get
        {
            if (_context == null)
            {
                throw new InvalidOperationException("The database context is not set. Please call SetPersistenceContext before using the repository.");
            }

            return _context;
        }
        private set => _context = value;
    }
    protected DbSet<TEntity> Entities
    {
        get
        {
            if (_entities == null)
            {
                throw new InvalidOperationException("The database context is not set. Please call SetPersistenceContext before using the repository.");
            }
            return _entities;
        }
        private set => _entities = value;
    }

    public void SetPersistenceContext(IPersistenceContext context)
    {
        using var _ = PersistenceTelemetry.ActivitySource.StartActivity("SetPersistenceContext", ActivityKind.Internal);

        if (context is not DbContext dbContext)
        {
            throw new ArgumentException("The provided context is not a valid DbContext.", nameof(context));
        }

        Context = dbContext;
        Entities = dbContext.Set<TEntity>();
    }
}
