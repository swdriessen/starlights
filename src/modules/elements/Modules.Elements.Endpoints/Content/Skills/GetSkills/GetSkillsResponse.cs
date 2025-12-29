namespace Starlights.Modules.Elements.Endpoints.Content.Skills.GetSkills;

public sealed record GetSkillsResponse(IReadOnlyList<SkillListItem> Skills);