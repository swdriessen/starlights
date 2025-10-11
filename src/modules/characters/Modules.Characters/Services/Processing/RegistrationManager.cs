using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public sealed class RegistrationManager : IRegistrationManager
{
    private readonly ILogger<RegistrationManager> _logger;
    private readonly IPersistence _persistence;
    private readonly List<IRegistrationBehavior> _behaviors;

    public RegistrationManager(ILogger<RegistrationManager> logger, IPersistence persistence, IEnumerable<IRegistrationBehavior> behaviors)
    {
        _logger = logger;
        _persistence = persistence;
        _behaviors = [.. behaviors];
    }

    public async Task Register(Registration newRegistration)
    {
        using var activity = CharactersInstrumentation.StartActivity(nameof(Register));

        _logger.LogInformation("Registering new registration '{ElementName} ({ElementType})'", newRegistration.AssociatedElementName, newRegistration.AssociatedElementType);

        foreach (var behavior in _behaviors)
        {
            await behavior.Registered(newRegistration);
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        registrations.Add(newRegistration);
    }

    public async Task Unregister(Registration existingRegistration)
    {
        using var activity = CharactersInstrumentation.StartActivity(nameof(Unregister));

        _logger.LogInformation("Unregistering registration '{ElementName} ({ElementType})'", existingRegistration.AssociatedElementName, existingRegistration.AssociatedElementType);

        var registrations = _persistence.GetRepository<IRegistrationRepository>();

        // Load all registrations for the character to determine subtree
        var allCharacterRegistrations = await registrations.GetRegistrationsAsync(existingRegistration.CharacterId);

        // Build parent -> children lookup
        var childrenByParent = allCharacterRegistrations
            .Where(r => r.ParentRegistrationId is not null)
            .GroupBy(r => r.ParentRegistrationId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Collect full set of registrations affected by this unregister (root + descendants)
        var registrationsToUnregister = BuildRegistrationsSubtree(existingRegistration, childrenByParent);
        registrationsToUnregister.Reverse(); // leaf-first (children before parents)

        foreach (var reg in registrationsToUnregister)
        {

            foreach (var behavior in _behaviors)
            {
                await behavior.Unregister(reg);
            }

            _logger.LogInformation("deleting registration {RegistrationId} ({ElementName})", reg.Id.Value, reg.AssociatedElementName);

            await registrations.DeleteRegistrationAsync(reg.Id);
            reg.MarkDeleted();
        }

        // mark character as re-processing required (some flag and event raised)
        // then a handler will pick this up and re-process all remaining registrations
    }

    private static List<Registration> BuildRegistrationsSubtree(Registration rootRegistration, Dictionary<RegistrationId, List<Registration>> childrenByParent)
    {
        var subtreeRegistrations = new List<Registration> { rootRegistration };
        var nodesPendingTraversal = new Stack<Registration>();
        nodesPendingTraversal.Push(rootRegistration);

        while (nodesPendingTraversal.Count > 0)
        {
            var currentRegistration = nodesPendingTraversal.Pop();

            if (childrenByParent.TryGetValue(currentRegistration.Id, out var childRegistrations))
            {
                foreach (var child in childRegistrations)
                {
                    subtreeRegistrations.Add(child);
                    nodesPendingTraversal.Push(child);
                }
            }
        }

        return subtreeRegistrations;
    }
}
