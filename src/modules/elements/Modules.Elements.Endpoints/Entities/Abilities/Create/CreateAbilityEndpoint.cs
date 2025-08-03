using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.Create;

public class CreateAbilityEndpoint : Endpoint<CreateAbilityRequest, CreateAbilityResponse>
{
    private readonly ILogger<CreateAbilityEndpoint> _logger;
    private readonly IPersistence _persistence;

    public CreateAbilityEndpoint(ILogger<CreateAbilityEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/abilities/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }
    public override async Task HandleAsync(CreateAbilityRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating a new ability with name: {Name} and abbreviation: {Abbreviation}", req.Name, req.Abbreviation);

        var element = Element.Create(req.Name, ElementTypeConstants.Ability);
        element.AddComponent(new AbbreviationComponent(element.Id, req.Abbreviation));

        var repository = _persistence.GetRepository<IElementsRepository>();

        repository.Add(element);

        var rows = await _persistence.SaveChangesAsync();

        if (rows == 0)
        {
            _logger.LogError("Failed to create ability. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        _logger.LogInformation("Successfully created ability with ID: {Id}", element.Id);

        var response = new CreateAbilityResponse
        {
            Id = Guid.NewGuid() // Simulate creation of a new ability
        };

        await Send.CreatedAtAsync("/api/elements/abilities/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
