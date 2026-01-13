namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.GetById;

public sealed record GetSpellByIdRequest
{
    public Guid Id { get; init; }
}
