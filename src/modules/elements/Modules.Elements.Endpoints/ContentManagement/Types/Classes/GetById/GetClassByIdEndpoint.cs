using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.GetById;

public sealed class GetClassByIdEndpoint : Endpoint<GetClassByIdRequest, ClassDataModel>
{
    private readonly ILogger<GetClassByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetClassByIdEndpoint(ILogger<GetClassByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/classes/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetClassByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving class [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Class)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var aspects = element.GetRequiredComponent<ClassAspects>();
        var description = element.GetRequiredComponent<DescriptionComponent>();
        var shortDescription = element.GetComponent<ShortDescriptionComponent>();

        var response = new ClassDataModel
        {
            Id = element.Id,
            Name = element.Name,
            HitPointDie = aspects.HitDice.ToString(),
            ShortDescription = shortDescription?.Content,
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
