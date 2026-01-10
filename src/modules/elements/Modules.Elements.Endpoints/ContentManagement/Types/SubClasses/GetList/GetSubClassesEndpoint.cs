using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.GetList;

public sealed class GetSubClassesEndpoint : EndpointWithoutRequest<GetSubClassesResponse>
{
    private readonly ILogger<GetSubClassesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetSubClassesEndpoint(ILogger<GetSubClassesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/sub-classes");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving sub classes");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.SubClass);

        var items = elements
            .Select(element =>
            {
                var meta = element.GetRequiredComponent<MetaComponent>();

                return new SubClassDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    ParentId = meta.Parent?.Id ?? default,
                    ParentName = meta.Parent?.Name ?? string.Empty,
                    Description = "" // omitted in list view
                };
            })
            .OrderBy(x => x.ParentName)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetSubClassesResponse(items), cancellation: ct);
    }
}
