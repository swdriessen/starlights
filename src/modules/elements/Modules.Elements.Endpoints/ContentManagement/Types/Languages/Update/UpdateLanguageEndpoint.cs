using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Language;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Update;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Languages.Update;

public sealed class UpdateLanguageEndpoint : Endpoint<UpdateLanguageRequest, UpdateLanguageResponse>
{
    private readonly IPersistence _persistence;

    public UpdateLanguageEndpoint(IPersistence persistence)
    {
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
        Logger.LogInformation("updating language [id='{Id}']", request.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(request.Id);

        if (element is null || element.Type != ElementTypeConstants.Language)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(request.Name);

        element.UpdateComponent<DescriptionComponent>(component => component.UpdateContent(request.Description));

        element.UpdateComponent<LanguageAspect>(component =>
        {
            component.UpdateClassification(new LanguageClassification(request.Kind));

            if (request.Origin is not null)
            {
                component.UpdateOrigin(request.Origin);
            }
        });

        await _persistence.SaveChangesAsync();

        Logger.LogInformation("updated language [id='{Id}']", request.Id);

        await Send.OkAsync(new UpdateLanguageResponse(element.Id), cancellation: ct);
    }
}
