using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Proficiency;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.Update;

public sealed class UpdateProficiencyEndpoint : Endpoint<UpdateProficiencyRequest>
{
    private readonly IPersistence _persistence;

    public UpdateProficiencyEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/proficiencies/{id:guid}");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateProficiencyRequest req, CancellationToken ct)
    {
        if (Route<Guid>("id") != req.Id)
        {
            AddError(r => r.Id, "Route id must match request id.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var repository = _persistence.GetRepository<IElementsRepository>();

        var proficiency = await repository.GetElementAsync(req.Id);
        if (proficiency is null || proficiency.Type != ElementTypeConstants.Proficiency)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Logger.LogInformation("Updating proficiency {ProficiencyId}", req.Id);

        proficiency.UpdateName(req.Name);

        var attributes = proficiency.GetComponent<ProficiencyAspects>();


        var classification = new ProficiencyClassification(req.ProficiencyType);

        if (attributes is null)
        {
            proficiency.AddComponent(id => new ProficiencyAspects(id, classification));
        }
        else
        {
            proficiency.UpdateComponent<ProficiencyAspects>(c => c.UpdateClassification(classification));
        }

        var description = proficiency.GetComponent<DescriptionComponent>();
        if (description is null)
        {
            proficiency.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));
        }
        else
        {
            proficiency.UpdateComponent<DescriptionComponent>(c => c.UpdateContent(req.Description ?? string.Empty));
        }

        await _persistence.SaveChangesAsync();
        await Send.NoContentAsync(ct);
    }
}
