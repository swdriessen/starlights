namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.GetById;

public sealed record GetFeatByIdRequest
{
    public required Guid Id { get; init; }
}
