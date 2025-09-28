using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.CharacterSheet.GetFeatures;

/// <summary>
/// Returns registrations for a character that represent character features that should be shown on the character sheet.
/// Currently limited to Class, Class Feature and Subclass (if represented separately by element type "Subclass" or "Class Feature").
/// </summary>
internal sealed class GetFeaturesEndpoint : EndpointWithoutRequest<GetFeaturesResponse>
{
    private static readonly HashSet<string> _includedTypes =
    [
        "Class",
        "Class Feature",
        // Some systems model subclasses separately; include common label variants
        "Subclass",
        "Sub Class"
    ];

    private readonly IPersistence _persistence;

    public GetFeaturesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("{characterId:guid}/features");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetFeaturesEndpoint)} | {characterId}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var registrationsRepo = _persistence.GetRepository<IRegistrationRepository>();
        var registrations = await registrationsRepo.GetRegistrationsAsync(characterId);

        // Map to simple model and then build hierarchy similar to GetRegistrations endpoint but filtered
        var models = registrations
            .Where(r => _includedTypes.Contains(r.AssociatedElementType))
            .Select(r => new RegistrationDataModel
            {
                RegistrationId = r.Id,
                CharacterId = r.CharacterId,
                AssociatedElementId = r.AssociatedElementId,
                ParentRegistrationId = r.ParentRegistrationId,
                Name = r.AssociatedElementName,
                Type = r.AssociatedElementType,
                Children = []
            })
            .ToList();

        // We only return the flat list of filtered registrations. If later we want hierarchy, we can reconstruct similarly.
        // For now, group by type for convenience on the client.
        var response = new GetFeaturesResponse
        {
            Features = models
                .OrderBy(m => m.Type)
                .ThenBy(m => m.Name)
                .ToList()
        };

        await Send.OkAsync(response, ct);
    }
}
