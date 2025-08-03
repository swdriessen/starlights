using FastEndpoints;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Elements.Integration;

namespace Starlights.Modules.Characters.Endpoints.Queries.CreationOptions;

public sealed class GetCharacterCreationOptionsEndpoint : EndpointWithoutRequest<GetCharacterCreationOptionsResponse>
{
    private readonly IElementsModuleQueries _elementsQueries;

    public GetCharacterCreationOptionsEndpoint(IElementsModuleQueries elementsQueries)
    {
        _elementsQueries = elementsQueries;
    }

    public override void Configure()
    {
        Get("/creation-options");
        AllowAnonymous();
        Group<CharactersGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        var characterCreationElements = await _elementsQueries.GetCharacterCreationElements();

        var options = characterCreationElements.ConvertAll(element => new CharacterCreationOption
        {
            Id = element.ElementId,
            Name = element.Name,
            ShortDescription = element.ShortDescription
        });

        var response = new GetCharacterCreationOptionsResponse
        {
            Options = options
        };

        await Send.OkAsync(response, ct);
    }
}

public record GetCharacterCreationOptionsResponse
{
    public List<CharacterCreationOption> Options { get; init; } = [];
}

public record CharacterCreationOption
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
}