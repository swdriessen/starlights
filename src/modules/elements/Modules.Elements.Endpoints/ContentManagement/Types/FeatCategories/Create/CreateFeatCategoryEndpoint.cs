using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.FeatCategories.Create;

/// <summary>
/// Creates a new feat category element.
/// </summary>
public sealed class CreateFeatCategoryEndpoint : Endpoint<CreateFeatCategoryRequest, CreateFeatCategoryResponse>
{
    private readonly ILogger<CreateFeatCategoryEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateFeatCategoryEndpoint(ILogger<CreateFeatCategoryEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/feat-categories/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateFeatCategoryRequest req, CancellationToken ct)
    {
        _logger.LogInformation("creating a new feat category [name='{Name}']", req.Name);

        var element = Element.Create(req.Name, ElementTypeConstants.FeatCategory);
        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to create feat category. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("successfully created feat category with ID: {Id}", element.Id);

        var response = new CreateFeatCategoryResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/feat-categories/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
