namespace Starlights.Modules.Elements.Endpoints.Content.Elements.GetList;

public sealed record GetElementsRequest
{
    /// <summary>
    /// Optional element type filter.
    /// </summary>
    public string? Type { get; init; }
}
