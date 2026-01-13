namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;

public record CreateSubClassRequest
{
    public required string Name { get; set; }
    public required Guid ParentClassId { get; set; }
    public required string ParentClassName { get; set; }
    public string? Description { get; set; } = string.Empty;
}
