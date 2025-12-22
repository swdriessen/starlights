using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetList;

/// <summary>
/// Retrieves all feat categories.
/// </summary>
public sealed class GetFeatCategoriesEndpoint : EndpointWithoutRequest<GetFeatCategoriesResponse>
{
    private readonly ILogger<GetFeatCategoriesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetFeatCategoriesEndpoint(ILogger<GetFeatCategoriesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/feat-categories");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving feat categories");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.FeatCategory);

        var items = elements
            .Select(element =>
            {
                var description = element.GetRequiredComponent<DescriptionComponent>();

                return new FeatCategoryDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    Description = description.Content
                };
            })
            .OrderBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetFeatCategoriesResponse(items), cancellation: ct);
    }
}
