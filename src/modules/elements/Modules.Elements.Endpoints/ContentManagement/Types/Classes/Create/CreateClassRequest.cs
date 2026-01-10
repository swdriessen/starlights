namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;

public record CreateClassRequest
{
    public required string Name { get; set; }
    public required int HitPointDieSize { get; set; }
    public int HitPointDieAmount { get; set; } = 1;
    public required string Description { get; set; }
    public string? ShortDescription { get; set; }
}
