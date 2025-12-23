using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Feats.Create;

/// <summary>
/// Creates a new feat element.
/// </summary>
public sealed class CreateFeatEndpoint : Endpoint<CreateFeatRequest, CreateFeatResponse>
{
    private readonly ILogger<CreateFeatEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateFeatEndpoint(ILogger<CreateFeatEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/feats/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateFeatRequest req, CancellationToken ct)
    {
        _logger.LogInformation("creating a new feat [name='{Name}']", req.Name);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var categoryElement = await repository.GetElementAsync(req.CategoryId);

        if (categoryElement is null || categoryElement.Type != ElementTypeConstants.FeatCategory)
        {
            AddError(r => r.CategoryId, "Category not found.");
            await Send.ErrorsAsync(statusCode: 404, cancellation: ct);
            return;
        }

        var element = Element.Create(req.Name, ElementTypeConstants.Feat);

        element.AddComponent(id => new FeatAttributesComponent(id, categoryElement.Id, categoryElement.Name));
        element.AddComponent(id => new PrerequisitesComponent(id, req.Prerequisite ?? string.Empty));
        element.AddComponent(id => new RepeatableComponent(id, req.IsRepeatable));

        if (!string.IsNullOrWhiteSpace(req.ShortDescription))
        {
            element.AddComponent(id => new ShortDescriptionComponent(id, req.ShortDescription));
        }

        if (!string.IsNullOrWhiteSpace(req.Description))
        {
            element.AddComponent(id => new DescriptionComponent(id, req.Description));
        }

        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to create feat. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("successfully created feat with ID: {Id}", element.Id);

        var response = new CreateFeatResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/feats/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
