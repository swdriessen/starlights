using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Components.Proficiency;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.Create;

public sealed class CreateProficiencyEndpoint : Endpoint<CreateProficiencyRequest, CreateProficiencyResponse>
{
    private readonly IPersistence _persistence;

    public CreateProficiencyEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/proficiencies");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProficiencyRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Creating proficiency '{ProficiencyName}' of type '{ProficiencyType}'", req.Name, req.ProficiencyType);

        // check if type is one of ....
        var acceptedTypes = new[] { "Skill", "Saving Throw", "Weapon", "Armor", "Tool", "Vehicle" };
        if (!acceptedTypes.Contains(req.ProficiencyType))
        {
            Logger.LogWarning("Invalid proficiency type '{ProficiencyType}'", req.ProficiencyType);
            await Send.ErrorsAsync(statusCode: 400, cancellation: ct);
            return;
        }

        var repository = _persistence.GetRepository<IElementsRepository>();

        var classification = new ProficiencyClassification(req.ProficiencyType);


        var element = Element.Create(req.Name, ElementTypeConstants.Proficiency);
        element.AddComponent(id => new ProficiencyAspects(id, classification));
        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));

        if (req.GenerateRules) // generate rules based on type
        {
            Logger.LogInformation("Generating rules for proficiency '{ProficiencyName}'", req.Name);

            var c = element.AddComponent(id => new StatisticRuleComponent(id, $"{req.Name}:{req.ProficiencyType}:proficiency", "proficiency", 0));
            c.UpdateStackingBonus("proficiency");
            c.UpdateDisplayName("Proficiency");
        }

        repository.Add(element);
        var rows = await _persistence.SaveChangesAsync();

        if (rows == 0)
        {
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.CreatedAtAsync(
            "/api/elements/proficiencies/{id}",
            new { id = element.Id.Value },
            new CreateProficiencyResponse(element.Id.Value),
            cancellation: ct);
    }




}
