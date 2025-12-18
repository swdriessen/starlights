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

    public override async Task HandleAsync(CreateSpellRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating a new spell [name='{Name}']", request.Name);

        var element = Element.Create(request.Name, ElementTypeConstants.Spell);

        element.AddComponent(id =>
        {
            return new SpellAttributesComponent(id, request.Level, request.MagicSchool, request.CastingTime, request.Range, request.Duration);
        });

        element.UpdateComponent<SpellAttributesComponent>(component =>
        {
            component.UpdateIsConcentrationRequired(request.IsConcentration);
            component.UpdateIsRitual(request.IsRitual);
            component.UpdateHasSomaticComponent(request.HasSomatic);
            component.UpdateHasVerbalComponent(request.HasVerbal);
            component.UpdateMaterialComponent(request.HasMaterial, request.MaterialComponent);
        });

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            element.AddComponent(id => new DescriptionComponent(id, request.Description));
        }

        var repository = _persistence.GetRepository<IElementsRepository>();

        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("Failed to create spell. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("Successfully created spell with ID: {Id}", element.Id);

        var response = new CreateSpellResponse
        {
            Id = element.Id
        };

        await Send.CreatedAtAsync("/api/elements/spells/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
