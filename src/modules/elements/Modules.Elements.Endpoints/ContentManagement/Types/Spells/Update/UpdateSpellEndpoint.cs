using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.Update;

public sealed class UpdateSpellEndpoint : Endpoint<UpdateSpellRequest, UpdateSpellResponse>
{
    private readonly ILogger<UpdateSpellEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateSpellEndpoint(ILogger<UpdateSpellEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/spells/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateSpellRequest request, CancellationToken ct)
    {
        _logger.LogInformation("updating spell [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Spell)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(request.Name);

        element.UpdateComponent<DescriptionComponent>(component =>
        {
            component.UpdateContent(request.Description);
        });

        element.UpdateComponent<SpellAttributesComponent>(component =>
        {
            component.UpdateLevel(request.Level);
            component.UpdateMagicSchool(request.MagicSchool);
            component.UpdateCastingTime(request.CastingTime);
            component.UpdateRange(request.Range);
            component.UpdateDuration(request.Duration);
            component.UpdateIsConcentrationRequired(request.IsConcentration);
            component.UpdateIsRitual(request.IsRitual);
            component.UpdateHasSomaticComponent(request.HasSomatic);
            component.UpdateHasVerbalComponent(request.HasVerbal);
            component.UpdateMaterialComponent(request.HasMaterial, request.MaterialComponent);
        });

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            // NOTE: it is possible that an update request is made without any actual changes
            // this results in 0 rows affected

            _logger.LogError("Failed to update spell. No rows affected. [id='{Id}']", request.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateSpellResponse(element.Id), cancellation: ct);
    }
}
