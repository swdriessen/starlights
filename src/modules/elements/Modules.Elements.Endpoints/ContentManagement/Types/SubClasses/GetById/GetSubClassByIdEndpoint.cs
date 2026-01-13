using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.GetById;

public sealed class GetSubClassByIdEndpoint : Endpoint<GetSubClassByIdRequest, SubClassDataModel>
{
    private readonly ILogger<GetSubClassByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetSubClassByIdEndpoint(ILogger<GetSubClassByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/sub-classes/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetSubClassByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving sub class [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.SubClass)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var meta = element.GetRequiredComponent<MetaComponent>();
        var description = element.GetRequiredComponent<DescriptionComponent>();

        var response = new SubClassDataModel
        {
            Id = element.Id,
            Name = element.Name,
            ParentId = meta.Parent?.Id ?? default,
            ParentName = meta.Parent?.Name ?? string.Empty,
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
