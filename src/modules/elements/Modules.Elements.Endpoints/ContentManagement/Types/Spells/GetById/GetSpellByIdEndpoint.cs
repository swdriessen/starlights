using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.GetById;

public sealed class GetSpellByIdEndpoint : Endpoint<GetSpellByIdRequest, SpellDataModel>
{
    private readonly ILogger<GetSpellByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetSpellByIdEndpoint(ILogger<GetSpellByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/spells/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetSpellByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving spell [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Spell)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var attributes = element.GetRequiredComponent<SpellAttributesComponent>();
        var description = element.GetRequiredComponent<DescriptionComponent>();

        var response = new SpellDataModel
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
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
