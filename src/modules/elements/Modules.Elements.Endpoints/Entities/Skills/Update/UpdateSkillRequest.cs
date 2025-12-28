namespace Starlights.Modules.Elements.Endpoints.Entities.Skills.Update;

public sealed record UpdateSkillRequest(Guid Id, string Name, Guid AbilityId, string Description);