using FastEndpoints;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

public sealed class GetSkillsEndpoint : EndpointWithoutRequest<GetSkillsResponse>
{
    private readonly IPersistence _persistence;

    public GetSkillsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/skills");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var skills = await repository.GetElementsByTypeAsync(ElementTypeConstants.Skill);
        var abilities = await repository.GetElementsByTypeAsync(ElementTypeConstants.Ability);
        var abilitiesById = abilities.ToDictionary(x => x.Id, x => x.Name);

        var items = skills
            .Select(skill =>
            {
                var abilityId = skill.GetRequiredComponent<PrimaryAbilityComponent>().PrimaryAbility;
                abilitiesById.TryGetValue(abilityId, out var abilityName);

                var description = skill.GetComponent<DescriptionComponent>()?.Content ?? string.Empty;

                return new SkillListItem(
                    Id: skill.Id.Value,
                    Name: skill.Name,
                    AbilityId: abilityId.Value,
                    Ability: abilityName ?? string.Empty,
                    Description: description);
            })
            .ToList();

        await Send.OkAsync(new GetSkillsResponse(items), cancellation: ct);
    }
}
