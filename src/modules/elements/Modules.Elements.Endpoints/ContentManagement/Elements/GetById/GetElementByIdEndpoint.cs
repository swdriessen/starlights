using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.GetById;

/// <summary>
/// Retrieves a generic element by its identifier.
/// </summary>
public sealed class GetElementByIdEndpoint : Endpoint<GetElementByIdRequest, ElementDataModel>
{
    private readonly ILogger<GetElementByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetElementByIdEndpoint(ILogger<GetElementByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetElementByIdRequest req, CancellationToken ct)
    {
        _logger.LogInformation("retrieving element [id='{Id}']", req.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.Id);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var description = element.GetComponent<DescriptionComponent>();

        var response = new ElementDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Type = element.Type,
            Description = description?.Content ?? string.Empty
        };

        await Send.OkAsync(response, ct);
    }
}
