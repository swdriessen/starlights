using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;

/// <summary>
/// Retrieves a feat category by its identifier.
/// </summary>
public sealed class GetFeatCategoryByIdEndpoint : Endpoint<GetFeatCategoryByIdRequest, FeatCategoryDataModel>
{
    private readonly ILogger<GetFeatCategoryByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetFeatCategoryByIdEndpoint(ILogger<GetFeatCategoryByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/feat-categories/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetFeatCategoryByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving feat category [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.FeatCategory)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var description = element.GetRequiredComponent<DescriptionComponent>();

        var response = new FeatCategoryDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
