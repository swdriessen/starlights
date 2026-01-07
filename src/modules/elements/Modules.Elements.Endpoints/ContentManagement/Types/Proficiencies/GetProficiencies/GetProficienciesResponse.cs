namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.GetProficiencies;

public sealed record GetProficienciesResponse(IReadOnlyCollection<ProficiencyListItem> Items);
