using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.Create;

public sealed class CreateSkillEndpoint : Endpoint<CreateSkillRequest, CreateSkillResponse>
{
    private readonly IPersistence _persistence;

    public CreateSkillEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/skills");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateSkillRequest req, CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var ability = await repository.GetElementAsync(req.AbilityId);
        if (ability is null || ability.Type != ElementTypeConstants.Ability)
        {
            AddError(r => r.AbilityId, $"Ability '{req.AbilityId}' was not found.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        Logger.LogInformation("Creating skill '{SkillName}' for ability '{AbilityName}' ({AbilityId})", req.Name, ability.Name, ability.Id);

        var element = Element.Create(req.Name, ElementTypeConstants.Skill);
        element.AddComponent(id => new PrimaryAbilityComponent(id, ability.Id));
        element.AddComponent(id => new DescriptionComponent(id, string.Empty));

        repository.Add(element);
        var rows = await _persistence.SaveChangesAsync();

        if (rows == 0)
        {
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.CreatedAtAsync(
            "/api/elements/skills/{id}",
            new { id = element.Id.Value },
            new CreateSkillResponse(element.Id.Value),
            cancellation: ct);
    }
}
