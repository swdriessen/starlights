using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Create;

/// <summary>
/// Creates a new generic element.
/// </summary>
public sealed class CreateElementEndpoint : Endpoint<CreateElementRequest, CreateElementResponse>
{
    private readonly ILogger<CreateElementEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateElementEndpoint(ILogger<CreateElementEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateElementRequest req, CancellationToken ct)
    {
        _logger.LogInformation("creating a new element [type='{Type}', name='{Name}']", req.Type, req.Name);

        var element = Element.Create(req.Name, req.Type);

        if (req.Description is not null)
        {
            element.AddComponent(id => new DescriptionComponent(id, req.Description));
        }

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to create element. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("successfully created element with ID: {Id}", element.Id);

        var response = new CreateElementResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
