using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Languages.Create;

public sealed class CreateLanguageEndpoint : Endpoint<CreateLanguageRequest, CreateLanguageResponse>
{
    private readonly ILogger<CreateLanguageEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateLanguageEndpoint(ILogger<CreateLanguageEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/languages/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateLanguageRequest req, CancellationToken ct)
    {
        _logger.LogInformation("creating a new language [name='{Name}']", req.Name);

        var element = Element.Create(req.Name, ElementTypeConstants.Language);

        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        element.AddComponent(id => new LanguageComponent(id, req.Origin ?? string.Empty));

        element.UpdateComponent<LanguageComponent>(component =>
        {
            component.UpdateKind(req.Kind);
        });

        var repository = _persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to create language. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("successfully created language with ID: {Id}", element.Id);

        var response = new CreateLanguageResponse(element.Id);

        await Send.CreatedAtAsync("/api/elements/languages/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
