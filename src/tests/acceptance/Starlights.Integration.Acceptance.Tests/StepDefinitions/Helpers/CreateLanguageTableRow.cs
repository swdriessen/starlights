namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

internal sealed record CreateLanguageTableRow : IMarkdownDescriptionTableRow
{
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? Origin { get; set; }
    public string? Description { get; set; } = string.Empty;
}
