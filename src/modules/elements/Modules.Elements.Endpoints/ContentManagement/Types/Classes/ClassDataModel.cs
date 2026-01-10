namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;

public sealed record ClassDataModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string HitPointDie { get; init; }
    public string? ShortDescription { get; init; }
    public required string Description { get; init; }
}
