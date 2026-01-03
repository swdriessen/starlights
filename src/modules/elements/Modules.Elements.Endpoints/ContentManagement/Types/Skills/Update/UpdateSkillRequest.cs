namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.Update;

public sealed record UpdateSkillRequest(Guid Id, string Name, Guid AbilityId, string Description);