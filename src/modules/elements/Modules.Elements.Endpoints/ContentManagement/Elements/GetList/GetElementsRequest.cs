namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.GetList;

public sealed record GetElementsRequest
{
    /// <summary>
    /// Optional element type filter.
    /// </summary>
    public string? Type { get; init; }
}
