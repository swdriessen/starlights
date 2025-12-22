using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Languages.GetList;

public sealed class GetLanguagesEndpoint : EndpointWithoutRequest<GetLanguagesResponse>
{
    private readonly ILogger<GetLanguagesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetLanguagesEndpoint(ILogger<GetLanguagesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/languages");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("retrieving languages");

        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Language);

        var items = elements
            .Select(element =>
            {
                var language = element.GetRequiredComponent<LanguageComponent>();

                return new LanguageDataModel
                {
                    Id = element.Id,
                    Name = element.Name,
                    Kind = language.Kind,
                    Origin = language.Origin,
                    Description = string.Empty // Description is omitted in the list view
                };
            })
            .OrderBy(x => x.Name)
            .ToList();

        await Send.OkAsync(new GetLanguagesResponse(items), cancellation: ct);
    }
}
