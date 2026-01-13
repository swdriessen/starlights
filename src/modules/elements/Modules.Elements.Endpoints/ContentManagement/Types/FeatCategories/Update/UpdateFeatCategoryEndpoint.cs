using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.FeatCategories.Update;

/// <summary>
/// Updates an existing feat category.
/// </summary>
public sealed class UpdateFeatCategoryEndpoint : Endpoint<UpdateFeatCategoryRequest, UpdateFeatCategoryResponse>
{
    private readonly ILogger<UpdateFeatCategoryEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateFeatCategoryEndpoint(ILogger<UpdateFeatCategoryEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/feat-categories/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateFeatCategoryRequest request, CancellationToken ct)
    {
        _logger.LogInformation("updating feat category [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.FeatCategory)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(request.Name);

        element.UpdateComponent<DescriptionComponent>(component =>
        {
            component.UpdateContent(request.Description);
        });

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("Failed to update feat category. No rows affected. [id='{Id}']", request.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateFeatCategoryResponse(element.Id), cancellation: ct);
    }
}
