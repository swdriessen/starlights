using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;

public sealed class CreateClassEndpoint : Endpoint<CreateClassRequest, CreateClassResponse>
{
    private readonly IPersistence _persistence;

    public CreateClassEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/classes/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateClassRequest req, CancellationToken ct)
    {
        Logger.LogInformation("creating a new class [name='{Name}']", req.Name);

        var element = Element.Create(req.Name, ElementTypeConstants.Class);

        element.AddComponent(id => new DescriptionComponent(id, req.Description));

        if (req.ShortDescription is not null)
        {
            element.AddComponent(id => new ShortDescriptionComponent(id, req.ShortDescription));
        }

        var hitDice = new HitPointDie(req.HitPointDieSize, req.HitPointDieAmount);
        element.AddComponent(id => new ClassAspects(id, hitDice));

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create class. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateClassResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/classes/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
