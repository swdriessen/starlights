using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.GetList;

public sealed class GetClassFeaturesEndpoint : EndpointWithoutRequest<GetClassFeaturesResponse>
{
    private readonly ILogger<GetClassFeaturesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetClassFeaturesEndpoint(ILogger<GetClassFeaturesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/class-features");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving class features");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.ClassFeature);

        var items = elements
            .Select(element =>
            {
                var aspects = element.GetRequiredComponent<FeatureAspects>();
                var meta = element.GetRequiredComponent<MetaComponent>();

                return new ClassFeatureDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    Level = aspects.Level,
                    ParentId = meta.Parent?.Id ?? default,
                    ParentName = meta.Parent?.Name ?? string.Empty,
                    Description = "" // omitted in list view
                };
            })
            .OrderBy(x => x.ParentName)
            .ThenBy(x => x.Level)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetClassFeaturesResponse(items), cancellation: ct);
    }
}
