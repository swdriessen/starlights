using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

sealed class GetRegistrationsEndpoint : Endpoint<GetRegistrationsRequest, GetRegistrationsResponse>
{
    private readonly ILogger<GetRegistrationsEndpoint> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public GetRegistrationsEndpoint(ILogger<GetRegistrationsEndpoint> logger, IPersistence persistence, IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _elements = elements;
    }

    public override void Configure()
    {
        Get("{characterId:guid}/registrations");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRegistrationsRequest req, CancellationToken ct)
    {
        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();

        var registrations = await registrationRepository.GetRegistrationsAsync(new(req.CharacterId));

        var models = registrations.ConvertAll(r => new RegistrationDataModel
        {
            RegistrationId = r.Id,
            CharacterId = r.CharacterId,
            AssociatedElementId = r.AssociatedElementId,
            ParentRegistrationId = r.ParentRegistrationId,
            Name = r.AssociatedElementName,
            Type = r.AssociatedElementType,
            Children = []
        });



        await Send.OkAsync(new GetRegistrationsResponse { Registrations = BuildHierarchy(models) }, ct);
    }

    private List<RegistrationDataModel> BuildHierarchy(List<RegistrationDataModel> registrations)
    {
        // Build lookup of all registrations
        var registrationDict = registrations.ToDictionary(r => r.RegistrationId, r =>
        {
            // Ensure children list is initialized (clone with a fresh children list to avoid side-effects if reused)
            r.Children = [];
            return r;
        });

        // Wire up children
        var roots = new List<RegistrationDataModel>();

        foreach (var registration in registrations)
        {
            if (registration.ParentRegistrationId is { } parentId &&
                registrationDict.TryGetValue(parentId, out var parentModel))
            {
                parentModel.Children.Add(registration);
            }
            else
            {
                roots.Add(registration);
            }
        }

        // Recursively process hierarchy, pruning/promoting as required
        var finalRoots = new List<RegistrationDataModel>();
        foreach (var root in roots)
        {
            finalRoots.AddRange(ProcessNode(root));
        }

        return finalRoots;

        // Returns a (possibly empty) list of nodes to represent this node in the
        // processed hierarchy. If the node is excluded, its (processed) children
        // are promoted. If included, it (with transformed children) is returned.
        IEnumerable<RegistrationDataModel> ProcessNode(RegistrationDataModel node)
        {
            if (node.Children.Count == 0)
            {
                if (ShouldIncludeInHierarchy(node.Type))
                {
                    return [node];
                }
                // Excluded leaf: drop entirely
                return [];
            }

            // Process children first
            var processedChildren = new List<RegistrationDataModel>();
            foreach (var child in node.Children)
            {
                processedChildren.AddRange(ProcessNode(child));
            }

            if (ShouldIncludeInHierarchy(node.Type))
            {
                node.Children = processedChildren;
                return [node];
            }

            // Node excluded -> promote its (already processed) children
            return processedChildren;
        }

        static bool ShouldIncludeInHierarchy(string type)
        {
            if (type is "Rule")
            {
                return false;
            }

            if (type is "Ability" or "Skill" or "Saving Throw")
            {
                return false;
            }

            return true;
            // Only these element types should appear directly; all others get promoted/removed
            return type is "Class" or "Class Feature" or "Background" or "Background Feature";
        }
    }

}


