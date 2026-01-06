using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Feat;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.GetList;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Feats.GetList;

/// <summary>
/// Retrieves all feats.
/// </summary>
public sealed class GetFeatsEndpoint : EndpointWithoutRequest<GetFeatsResponse>
{
    private readonly ILogger<GetFeatsEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetFeatsEndpoint(ILogger<GetFeatsEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/feats");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving feats");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Feat);

        var items = elements
            .Select(element =>
            {
                var attributes = element.GetRequiredComponent<FeatAspects>();
                var prerequisites = element.GetComponent<PrerequisitesComponent>();
                var repeatable = element.GetComponent<RepeatableComponent>();
                var shortDescription = element.GetComponent<ShortDescriptionComponent>();

                return new FeatDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    CategoryId = attributes.Category.Id.Value,
                    Category = attributes.Category.Name,
                    ShortDescription = shortDescription?.Content ?? string.Empty,
                    Description = string.Empty, // Description is omitted in the list view
                    Prerequisites = prerequisites?.Prerequisites ?? string.Empty,
                    IsRepeatable = repeatable?.IsRepeatable ?? false
                };
            })
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetFeatsResponse(items), ct);
    }
}
