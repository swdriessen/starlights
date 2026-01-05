using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components.Spellcasting;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.GetList;

public sealed class GetSpellsEndpoint : EndpointWithoutRequest<GetSpellsResponse>
{
    private readonly ILogger<GetSpellsEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetSpellsEndpoint(ILogger<GetSpellsEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/spells");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving spells");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Spell);

        var items = elements
            .Select(element =>
            {
                var attributes = element.GetRequiredComponent<SpellcastingAspects>();

                return new SpellDataModel()
                {
                    Id = element.Id,
                    Name = element.Name,
                    Level = attributes.Classification.Level,
                    MagicSchool = attributes.Classification.MagicSchool,
                    CastingTime = attributes.CastingTime.Value,
                    Range = attributes.Range.ToString(),
                    Duration = attributes.Duration.Value,
                    IsConcentration = attributes.Duration.IsConcentration,
                    IsRitual = attributes.CastingTime.IsRitual,
                    HasSomatic = attributes.Components.HasSomatic,
                    HasVerbal = attributes.Components.HasVerbal,
                    HasMaterial = attributes.Components.HasMaterial,
                    MaterialComponent = attributes.Components.MaterialComponent,
                    Description = "" // Description is omitted in the list view
                };
            })
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetSpellsResponse(items), cancellation: ct);
    }
}
