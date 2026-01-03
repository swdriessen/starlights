using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Update;

public sealed class UpdateFeatEndpoint : Endpoint<UpdateFeatRequest, UpdateFeatResponse>
{
    private readonly ILogger<UpdateFeatEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateFeatEndpoint(ILogger<UpdateFeatEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/feats/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateFeatRequest request, CancellationToken ct)
    {
        _logger.LogInformation("updating feat [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Feat)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var categoryElement = await repository.GetElementAsync(request.CategoryId);
        if (categoryElement is null || categoryElement.Type != ElementTypeConstants.FeatCategory)
        {
            AddError(r => r.CategoryId, "Category not found.");
            await Send.ErrorsAsync(statusCode: 404, cancellation: ct);
            return;
        }

        element.UpdateName(request.Name);

        element.UpdateComponent<FeatAttributesComponent>(component =>
        {
            component.UpdateCategory(categoryElement.Id, categoryElement.Name);
        });

        element.UpdateComponent<PrerequisitesComponent>(component =>
        {
            component.UpdatePrerequisites(request.Prerequisite ?? string.Empty);
        });

        element.UpdateComponent<RepeatableComponent>(component =>
        {
            component.SetRepeatable(request.IsRepeatable);
        });

        element.UpdateComponent<DescriptionComponent>(component =>
        {
            component.UpdateContent(request.Description);
        });

        if (request.ShortDescription != null)
        {
            element.UpdateComponent<ShortDescriptionComponent>(component =>
            {
                component.UpdateContent(request.ShortDescription);
            });
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("Failed to update feat. No rows affected. [id='{Id}']", request.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateFeatResponse(element.Id), cancellation: ct);
    }
}
