using Starlights.Modules.Characters.Endpoints.Characters.Skills;

namespace Starlights.Modules.Characters.Endpoints.Characters.Skills.GetSkills;

internal sealed class GetSkillsResponse
{
    public List<SkillDataModel> Skills { get; set; } = [];
}
