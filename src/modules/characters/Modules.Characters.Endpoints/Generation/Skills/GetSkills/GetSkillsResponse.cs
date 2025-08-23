using Starlights.Modules.Characters.Endpoints.Generation.Skills;

namespace Starlights.Modules.Characters.Endpoints.Generation.Skills.GetSkills;

internal sealed class GetSkillsResponse
{
    public List<SkillDataModel> Skills { get; set; } = [];
}
