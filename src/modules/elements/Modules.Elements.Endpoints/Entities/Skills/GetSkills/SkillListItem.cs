namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

public sealed record SkillListItem(Guid Id, string Name, Guid AbilityId, string Ability, string Description);