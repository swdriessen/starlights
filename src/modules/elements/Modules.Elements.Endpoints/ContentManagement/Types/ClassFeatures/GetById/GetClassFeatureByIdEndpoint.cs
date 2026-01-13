using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.GetById;

public sealed class GetClassFeatureByIdEndpoint : Endpoint<GetClassFeatureByIdRequest, ClassFeatureDataModel>
{
    private readonly ILogger<GetClassFeatureByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetClassFeatureByIdEndpoint(ILogger<GetClassFeatureByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/class-features/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetClassFeatureByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving class feature [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.ClassFeature)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var aspects = element.GetRequiredComponent<FeatureAspects>();
        var meta = element.GetRequiredComponent<MetaComponent>();
        var description = element.GetRequiredComponent<DescriptionComponent>();

        var response = new ClassFeatureDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Level = aspects.Level,
            ParentId = meta.Parent?.Id ?? default,
            ParentName = meta.Parent?.Name ?? string.Empty,
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
