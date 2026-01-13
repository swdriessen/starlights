using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.GetList;

public sealed class GetClassesEndpoint : EndpointWithoutRequest<GetClassesResponse>
{
    private readonly ILogger<GetClassesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetClassesEndpoint(ILogger<GetClassesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/classes");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving classes");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Class);

        var items = elements
            .Select(element =>
            {
                var aspects = element.GetRequiredComponent<ClassAspects>();
                var shortDescription = element.GetComponent<ShortDescriptionComponent>();

                return new ClassDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    HitPointDie = aspects.HitDice.ToString(),
                    ShortDescription = shortDescription?.Content,
                    Description = "" // omitted in list view
                };
            })
            .OrderBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetClassesResponse(items), cancellation: ct);
    }
}
