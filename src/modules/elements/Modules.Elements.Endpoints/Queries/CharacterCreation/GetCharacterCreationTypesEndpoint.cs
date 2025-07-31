using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Integration.Abstractions;

namespace Starlights.Modules.Elements.Endpoints.Queries.CharacterCreation;

public class GetCharacterCreationTypesEndpoint : EndpointWithoutRequest
{
    private readonly ILogger<GetCharacterCreationTypesEndpoint> _logger;
    private readonly IElementsModuleQueries _queries;

    public GetCharacterCreationTypesEndpoint(ILogger<GetCharacterCreationTypesEndpoint> logger, IElementsModuleQueries queries)
    {
        _logger = logger;
        _queries = queries;
    }

    public override void Configure()
    {
        Get("/elements/types/character-creation");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elements = _queries.GetCharacterCreationElements();
        await Send.OkAsync(elements, ct);
    }
}
