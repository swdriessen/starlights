using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;

public sealed class CreateClassFeatureEndpoint : Endpoint<CreateClassFeatureRequest, CreateClassFeatureResponse>
{
    private readonly IPersistence _persistence;

    public CreateClassFeatureEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/class-features/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateClassFeatureRequest req, CancellationToken ct)
    {
        Logger.LogInformation(
            "creating a new class feature [name='{Name}', parentClassId='{ParentClassId}', level={Level}]",
            req.Name,
            req.ParentClassId,
            req.Level);

        var element = Element.Create(req.Name, ElementTypeConstants.ClassFeature);

        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        var parent = new ParentElement(new ElementId { Value = req.ParentClassId }, req.ParentClassName);
        element.AddComponent(id => new MetaComponent(id, parent));

        element.AddComponent(id => new FeatureAspects(id, req.Level));

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create class feature. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateClassFeatureResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/class-features/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
