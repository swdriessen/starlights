using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Create;

public sealed class CreateSavingThrowEndpoint : Endpoint<CreateSavingThrowRequest, CreateSavingThrowResponse>
{
    private readonly IPersistence _persistence;

    public CreateSavingThrowEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/saving-throws");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateSavingThrowRequest req, CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var abilityElement = await repository.GetElementAsync(req.AbilityId);
        if (abilityElement is null || abilityElement.Type != ElementTypeConstants.Ability)
        {
            AddError(r => r.AbilityId, $"Ability '{req.AbilityId}' was not found.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        Logger.LogInformation(
            "Creating saving throw '{SavingThrowName}' for ability '{AbilityName}' ({AbilityId})",
            req.Name,
            abilityElement.Name,
            abilityElement.Id);

        var element = Element.Create(req.Name, ElementTypeConstants.SavingThrow);
        element.AddComponent(id => new PrimaryAbilityComponent(id, abilityElement.Id));
        element.AddComponent(id => new DescriptionComponent(id, string.Empty));

        repository.Add(element);
        var rows = await _persistence.SaveChangesAsync();

        if (rows == 0)
        {
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.CreatedAtAsync(
            "/api/elements/saving-throws/{id}",
            new { id = element.Id.Value },
            new CreateSavingThrowResponse(element.Id.Value),
            cancellation: ct);
    }
}