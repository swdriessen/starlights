using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Entities.Skills.GetSkills;

internal sealed class GetSkillsRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}
