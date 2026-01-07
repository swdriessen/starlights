namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.Update;

public sealed record UpdateProficiencyRequest(Guid Id, string Name, string ProficiencyType, string? Description);
