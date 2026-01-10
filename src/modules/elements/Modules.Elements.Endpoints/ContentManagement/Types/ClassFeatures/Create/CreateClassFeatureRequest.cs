namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;

public record CreateClassFeatureRequest
{
    public required string Name { get; set; }
    public required Guid ParentClassId { get; set; }
    public required string ParentClassName { get; set; }
    public int Level { get; set; }
    public string? Description { get; set; } = string.Empty;
}
