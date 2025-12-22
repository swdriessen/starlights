namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

/// <summary>
/// A marker interface for a class that represents a table row
/// </summary>
internal interface ITableRow;

/// <summary>
/// Represents a table row that may contain a markdown description placeholder.
/// </summary>
internal interface IMarkdownDescriptionTableRow : ITableRow
{
    string? Description { get; set; }
}
