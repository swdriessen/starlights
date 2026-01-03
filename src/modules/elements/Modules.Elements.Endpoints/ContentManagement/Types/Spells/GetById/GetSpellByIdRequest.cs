namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.GetById;

public sealed record GetSpellByIdRequest
{
    public Guid Id { get; init; }
}
