namespace Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.Create;

public sealed record CreateProficiencyRequest(string Name, string ProficiencyType, string? Description)
{
    /// <summary>
    /// Whether to automatically generate associated statistic rules for the proficiency.
    /// </summary>
    public bool GenerateRules { get; init; } = true;
}
