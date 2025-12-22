namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

internal sealed record UpdateLanguageTableRow : IMarkdownDescriptionTableRow
{
    public string? Name { get; set; }
    public string? Kind { get; set; }
    public string? Origin { get; set; }
    public string? Description { get; set; }
}
