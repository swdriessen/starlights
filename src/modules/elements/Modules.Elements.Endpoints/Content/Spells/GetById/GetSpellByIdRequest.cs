namespace Starlights.Modules.Elements.Endpoints.Content.Spells.GetById;

public sealed record GetSpellByIdRequest
{
    public Guid Id { get; init; }
}
