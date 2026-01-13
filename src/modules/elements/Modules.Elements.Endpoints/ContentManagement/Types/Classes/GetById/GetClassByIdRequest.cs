namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.GetById;

public sealed record GetClassByIdRequest
{
    public Guid Id { get; init; }
}
