using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Delete;

public sealed class DeleteSavingThrowEndpoint : EndpointWithoutRequest
{
    private readonly IPersistence _persistence;

    public DeleteSavingThrowEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/saving-throws/{id:guid}");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var repository = _persistence.GetRepository<IElementsRepository>();

        var existing = await repository.GetElementAsync(id);
        if (existing is null || existing.Type != ElementTypeConstants.SavingThrow)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Logger.LogInformation("Deleting saving throw {SavingThrowId}", id);

        var deleted = await repository.DeleteElementAsync(id);
        if (!deleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await _persistence.SaveChangesAsync();
        await Send.NoContentAsync(ct);
    }
}