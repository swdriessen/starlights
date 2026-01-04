using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Aspects;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.Create;

public class CreateSpellEndpoint : Endpoint<CreateSpellRequest, CreateSpellResponse>
{
    private readonly IPersistence _persistence;

    public CreateSpellEndpoint(IPersistence persistence)
    {
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
        Logger.LogInformation("creating a new spell [name='{Name}']", req.Name);

        var element = Element.Create(req.Name, ElementTypeConstants.Spell);

        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        var classification = new SpellClassification(req.MagicSchool, req.Level);
        var time = new CastingTime(req.CastingTime) { IsRitual = req.IsRitual };
        var range = new SpellcastingRange(req.Range);
        var duration = new Duration(req.Duration, req.IsConcentration);
        var components = new SpellComponents()
        {
            HasVerbal = req.HasVerbal,
            HasSomatic = req.HasSomatic,
            HasMaterial = req.HasMaterial,
            MaterialComponent = req.MaterialComponent
        };

        element.AddComponent(id => new SpellcastingAspects(id, classification, time, range, duration));

        element.UpdateComponent<SpellcastingAspects>(component =>
        {
            component.UpdateHasSomaticComponent(components.HasSomatic);
            component.UpdateHasVerbalComponent(components.HasVerbal);
            component.UpdateMaterialComponent(components.HasMaterial, components.MaterialComponent);
        });

        var repository = _persistence.GetRepository<IElementsRepository>();

        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create spell. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        Logger.LogInformation("successfully created spell with ID: {Id}", element.Id);

        var response = new CreateSpellResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/spells/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
