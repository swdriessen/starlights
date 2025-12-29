using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Update;

public sealed class UpdateSavingThrowEndpoint : Endpoint<UpdateSavingThrowRequest>
{
    private readonly IPersistence _persistence;

    public UpdateSavingThrowEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/saving-throws/{id:guid}");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateSavingThrowRequest req, CancellationToken ct)
    {
        if (Route<Guid>("id") != req.Id)
        {
            AddError(r => r.Id, "Route id must match request id.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var repository = _persistence.GetRepository<IElementsRepository>();

        var savingThrow = await repository.GetElementAsync(req.Id);
        if (savingThrow is null || savingThrow.Type != ElementTypeConstants.SavingThrow)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var ability = await repository.GetElementAsync(req.AbilityId);
        if (ability is null || ability.Type != ElementTypeConstants.Ability)
        {
            AddError(r => r.AbilityId, $"Ability '{req.AbilityId}' was not found.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        Logger.LogInformation("Updating saving throw {SavingThrowId}", req.Id);

        savingThrow.UpdateName(req.Name);
        savingThrow.UpdateComponent<PrimaryAbilityComponent>(c => c.UpdatePrimaryAbility(ability.Id));

        var description = savingThrow.GetComponent<DescriptionComponent>();
        if (description is null)
        {
            savingThrow.AddComponent(id => new DescriptionComponent(id, req.Description));
        }
        else
        {
            savingThrow.UpdateComponent<DescriptionComponent>(c => c.UpdateContent(req.Description));
        }

        await _persistence.SaveChangesAsync();
        await Send.NoContentAsync(ct);
    }
}