namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;

public sealed record ClassFeatureDataModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required string ParentName { get; init; }
    public required Guid ParentId { get; init; }
    public required string Description { get; init; }
}
