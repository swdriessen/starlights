using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.Create;

/// <summary>
/// Creates a new generic element.
/// </summary>
public sealed class CreateElementEndpoint : Endpoint<CreateElementRequest, CreateElementResponse>
{
    private readonly IPersistence _persistence;

    public CreateElementEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateElementRequest req, CancellationToken ct)
    {
        Logger.LogInformation("creating a new element [type='{Type}', name='{Name}']", req.Type, req.Name);

        var newElement = Element.Create(req.Name, req.Type);

        newElement.AddComponent(el => new DescriptionComponent(el, req.Description ?? ""));

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(newElement);

        await _persistence.SaveChangesAsync();

        Logger.LogInformation("successfully created element with ID: {Id}", newElement.Id);

        var response = new CreateElementResponse(newElement.Id);

        await Send.CreatedAtAsync("/api/elements/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
