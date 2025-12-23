namespace Starlights.Modules.Elements.Endpoints.Content.Feats.GetById;

public sealed record GetFeatByIdRequest
{
    public required Guid Id { get; init; }
}
