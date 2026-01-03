using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.GetList;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Attributes.Spells.GetList;

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
                var attributes = element.GetRequiredComponent<SpellAttributesComponent>();

                return new SpellDataModel()
                {
                    Id = element.Id,
                    Name = element.Name,
                    Level = attributes.Level,
                    MagicSchool = attributes.MagicSchool,
                    CastingTime = attributes.CastingTime,
                    Range = attributes.Range,
                    Duration = attributes.Duration,
                    IsConcentration = attributes.IsConcentrationRequired,
                    IsRitual = attributes.IsRitual,
                    HasSomatic = attributes.HasSomaticComponent,
                    HasVerbal = attributes.HasVerbalComponent,
                    HasMaterial = attributes.HasMaterialComponent,
                    MaterialComponent = attributes.MaterialComponentsDescription,
                    Description = "" // Description is omitted in the list view
                };
            })
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetSpellsResponse(items), cancellation: ct);
    }
}
