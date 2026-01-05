using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Language;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.GetById;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Languages.GetById;

public sealed class GetLanguageByIdEndpoint : Endpoint<GetLanguageByIdRequest, LanguageDataModel>
{
    private readonly ILogger<GetLanguageByIdEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetLanguageByIdEndpoint(ILogger<GetLanguageByIdEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/languages/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetLanguageByIdRequest request, CancellationToken ct)
    {
        _logger.LogInformation("retrieving language [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Language)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var language = element.GetRequiredComponent<LanguageAspect>();
        var description = element.GetRequiredComponent<DescriptionComponent>();

        var response = new LanguageDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Kind = language.Classification,
            Origin = language.Origin,
            Description = description.Content
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
