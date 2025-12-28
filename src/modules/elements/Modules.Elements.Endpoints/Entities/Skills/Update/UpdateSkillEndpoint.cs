using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.Update;

public sealed class UpdateSkillEndpoint : Endpoint<UpdateSkillRequest>
{
    private readonly IPersistence _persistence;

    public UpdateSkillEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/skills/{id:guid}");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateSkillRequest req, CancellationToken ct)
    {
        if (Route<Guid>("id") != req.Id)
        {
            AddError(r => r.Id, "Route id must match request id.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var repository = _persistence.GetRepository<IElementsRepository>();

        var skill = await repository.GetElementAsync(req.Id);
        if (skill is null || skill.Type != ElementTypeConstants.Skill)
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

        Logger.LogInformation("Updating skill {SkillId}", req.Id);

        skill.UpdateName(req.Name);
        skill.UpdateComponent<PrimaryAbilityComponent>(c => c.UpdatePrimaryAbility(ability.Id));

        var description = skill.GetComponent<DescriptionComponent>();
        if (description is null)
        {
            skill.AddComponent(id => new DescriptionComponent(id, req.Description));
        }
        else
        {
            skill.UpdateComponent<DescriptionComponent>(c => c.UpdateContent(req.Description));
        }

        await _persistence.SaveChangesAsync();
        await Send.NoContentAsync(ct);
    }
}
