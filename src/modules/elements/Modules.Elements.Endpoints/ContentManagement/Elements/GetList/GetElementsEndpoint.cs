using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.GetList;

/// <summary>
/// Retrieves elements (optionally filtered by type).
/// </summary>
public sealed class GetElementsEndpoint : Endpoint<GetElementsRequest, GetElementsResponse>
{
    private readonly ILogger<GetElementsEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetElementsEndpoint(ILogger<GetElementsEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetElementsRequest req, CancellationToken ct)
    {
        _logger.LogInformation("retrieving elements [type='{Type}']", req.Type);

        var repository = _persistence.GetRepository<IElementsRepository>();

        var elements = string.IsNullOrWhiteSpace(req.Type)
            ? await repository.GetElementsAsync()
            : await repository.GetElementsByTypeAsync(req.Type);

        var items = elements
            .Select(element =>
            {
                var description = element.GetComponent<DescriptionComponent>();

                return new ElementDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    Type = element.Type,
                    Description = description?.Content ?? string.Empty
                };
            })
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetElementsResponse(items), ct);
    }
}
