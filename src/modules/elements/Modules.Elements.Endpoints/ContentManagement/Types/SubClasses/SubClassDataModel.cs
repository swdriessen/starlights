namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;

public sealed record SubClassDataModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid ParentId { get; init; }
    public required string ParentName { get; init; }
    public required string Description { get; init; }
}
