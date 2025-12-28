using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.Delete;

public sealed class DeleteProficiencyEndpoint : EndpointWithoutRequest
{
    private readonly IPersistence _persistence;

    public DeleteProficiencyEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/proficiencies/{id:guid}");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var repository = _persistence.GetRepository<IElementsRepository>();

        var existing = await repository.GetElementAsync(id);
        if (existing is null || existing.Type != ElementTypeConstants.Proficiency)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Logger.LogInformation("Deleting proficiency {ProficiencyId}", id);

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
