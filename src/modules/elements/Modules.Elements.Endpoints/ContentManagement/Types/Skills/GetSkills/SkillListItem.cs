namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.GetSkills;

public sealed record SkillListItem(Guid Id, string Name, Guid AbilityId, string Ability, string Description);