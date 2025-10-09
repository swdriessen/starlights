using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class RegistrationRepository : RepositoryBase<Registration>, IRegistrationRepository
{
    private readonly ILogger<RegistrationRepository> _logger;

    public RegistrationRepository(ILogger<RegistrationRepository> logger)
    {
        _logger = logger;
    }

    public void Add(Registration registration)
    {
        Entities.Add(registration);
    }

    public Task<bool> DeleteRegistrationAsync(RegistrationId id)
    {
        var toRemove = Entities.SingleOrDefault(r => r.Id == id);
        if (toRemove != null)
        {
            Entities.Remove(toRemove);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteRegistrationsAsync(CharacterId id)
    {
        var toRemove = Entities.Where(r => r.CharacterId == id);
        if (toRemove.Any())
        {
            Entities.RemoveRange(toRemove);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public async Task<Registration?> GetRegistrationAsync(RegistrationId id)
    {
        var registration = await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .SingleOrDefaultAsync(r => r.Id == id);

        if (registration is null)
        {
            registration = Entities.Local.OfType<Registration>().SingleOrDefault(r => r.Id == id);

            if (registration is not null)
            {
                _logger.LogWarning("Including pending registrations in local context.");
            }
        }

        return registration;
    }

    public async Task<List<Registration>> GetRegistrationsAsync(CharacterId id)
    {
        var registrations = await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .Where(r => r.CharacterId == id)
            .ToListAsync();

        var pending = Entities.Local
            .OfType<Registration>()
            .Where(r => r.CharacterId == id && registrations.All(existing => existing.Id != r.Id))
            .ToList();

        if (pending.Count > 0)
        {
            _logger.LogWarning("Including '{PendingCount}' pending registrations in local context.", pending.Count);
            registrations.AddRange(pending);
        }

        return registrations;
    }

    public async Task<List<Registration>> GetRegistrationsByAssociationsAsync(CharacterId id, ElementId associatedElementId)
    {
        var registrations = await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .Where(r => r.CharacterId == id && r.AssociatedElementId == associatedElementId)
            .ToListAsync();

        // include local entities that were added but untracked yet
        var pending = Entities.Local
            .OfType<Registration>()
            .Where(r => r.CharacterId == id
                && r.AssociatedElementId == associatedElementId
                && registrations.All(existing => existing.Id != r.Id))
            .ToList();

        if (pending.Count > 0)
        {
            _logger.LogWarning("Including '{PendingCount}' pending registrations in local context.", pending.Count);
            registrations.AddRange(pending);
        }

        return registrations;
    }
}