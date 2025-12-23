using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Feats.GetById;

/// <summary>
/// Retrieves a feat by its identifier.
/// </summary>
public sealed class GetFeatByIdEndpoint : Endpoint<GetFeatByIdRequest, FeatDataModel>
{
    private readonly ILogger<GetFeatByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetFeatByIdEndpoint(ILogger<GetFeatByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/feats/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetFeatByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving feat [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Feat)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var attributes = element.GetRequiredComponent<FeatAttributesComponent>();
        var prerequisites = element.GetRequiredComponent<PrerequisitesComponent>();
        var repeatable = element.GetRequiredComponent<RepeatableComponent>();
        var shortDescription = element.GetComponent<ShortDescriptionComponent>();
        var description = element.GetComponent<DescriptionComponent>();

        var response = new FeatDataModel
        {
            Id = element.Id,
            Name = element.Name,
            CategoryId = attributes.CategoryId,
            Category = attributes.Category,
            ShortDescription = shortDescription?.Content ?? string.Empty,
            Description = description?.Content ?? string.Empty,
            Prerequisites = prerequisites?.Prerequisites ?? string.Empty,
            IsRepeatable = repeatable?.IsRepeatable ?? false
        };

        await Send.OkAsync(response, ct);
    }
}
