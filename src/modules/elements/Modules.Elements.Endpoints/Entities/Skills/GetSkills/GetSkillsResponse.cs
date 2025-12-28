namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

public sealed record GetSkillsResponse(IReadOnlyList<SkillListItem> Skills);