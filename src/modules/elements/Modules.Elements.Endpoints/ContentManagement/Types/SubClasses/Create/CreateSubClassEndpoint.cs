using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;

public sealed class CreateSubClassEndpoint : Endpoint<CreateSubClassRequest, CreateSubClassResponse>
{
    private readonly IPersistence _persistence;

    public CreateSubClassEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/sub-classes/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateSubClassRequest req, CancellationToken ct)
    {
        Logger.LogInformation(
            "creating a new sub class [name='{Name}', ParentClassName='{ParentClassName}']",
            req.Name,
            req.ParentClassName);

        var element = Element.Create(req.Name, ElementTypeConstants.SubClass);

        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        var parent = new ParentElement(new ElementId { Value = req.ParentClassId }, req.ParentClassName);
        element.AddComponent(id => new MetaComponent(id, parent));

        var classifications = element.AddComponent(id => new ClassificationsComponent(id));
        classifications.AddLabel(parent.Name);

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create sub class. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateSubClassResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/sub-classes/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
