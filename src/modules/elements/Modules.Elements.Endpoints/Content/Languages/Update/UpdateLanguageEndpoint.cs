using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Languages.Update;

public sealed class UpdateLanguageEndpoint : Endpoint<UpdateLanguageRequest, UpdateLanguageResponse>
{
    private readonly ILogger<UpdateLanguageEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateLanguageEndpoint(ILogger<UpdateLanguageEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/languages/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateLanguageRequest request, CancellationToken ct)
    {
        _logger.LogInformation("updating language [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Language)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(request.Name);

        element.UpdateComponent<DescriptionComponent>(component =>
        {
            component.UpdateContent(request.Description);
        });

        element.UpdateComponent<LanguageComponent>(component =>
        {
            component.UpdateKind(request.Kind);
            component.UpdateOrigin(request.Origin ?? string.Empty);
        });

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("Failed to update language. No rows affected. [id='{Id}']", request.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateLanguageResponse(element.Id), cancellation: ct);
    }
}
