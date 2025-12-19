using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Spells.Create;

public class CreateSpellEndpoint : Endpoint<CreateSpellRequest, CreateSpellResponse>
{
    private readonly ILogger<CreateSpellEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateSpellEndpoint(ILogger<CreateSpellEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/spells/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateSpellRequest req, CancellationToken ct)
    {
        _logger.LogInformation("creating a new spell [name='{Name}']", req.Name);

        var element = Element.Create(req.Name, ElementTypeConstants.Spell);

        element.AddComponent(id => new DescriptionComponent(id, req.Description));

        element.AddComponent(id => new SpellAttributesComponent(id, req.Level, req.MagicSchool, req.CastingTime, req.Range, req.Duration));

        element.UpdateComponent<SpellAttributesComponent>(component =>
        {
            component.UpdateIsConcentrationRequired(req.IsConcentration);
            component.UpdateIsRitual(req.IsRitual);
            component.UpdateHasSomaticComponent(req.HasSomatic);
            component.UpdateHasVerbalComponent(req.HasVerbal);
            component.UpdateMaterialComponent(req.HasMaterial, req.MaterialComponent);
        });

        var repository = _persistence.GetRepository<IElementsRepository>();

        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to create spell. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("successfully created spell with ID: {Id}", element.Id);

        var response = new CreateSpellResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/spells/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
