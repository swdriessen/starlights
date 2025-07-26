using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Integration.Abstractions;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Application.Services;

internal class ElementsModuleGateway : IElementsModuleGateway
{
    private readonly ILogger<ElementsModuleGateway> _logger;
    private readonly IPersistence _persistence;

    public ElementsModuleGateway(ILogger<ElementsModuleGateway> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public async Task<ElementModel?> GetElement(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

        _logger.LogInformation("Retrieving element with identifier: {Identifier}", identifier);

        var repo = _persistence.GetRepository<IElementsRepository>();

        var element = await repo.GetElementAsync(Guid.Parse(identifier));

        if (element is null)
        {
            _logger.LogWarning("Element with identifier {Identifier} not found.", identifier);
            return default;
        }

        _logger.LogInformation("Element with identifier {Identifier} retrieved successfully.", identifier);

        return new ElementModel(element.Name, element.Type, "Internal", element.Id);
    }

    public async Task<IEnumerable<ElementModel>> GetElements(string type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        _logger.LogInformation("Retrieving elements of type: {ElementType}", type);

        var repo = _persistence.GetRepository<IElementsRepository>();

        var elements = await repo.GetElementsByTypeAsync(type);

        if (!elements.Any())
        {
            _logger.LogWarning("No elements found of type {ElementType}.", type);
            return [];
        }

        _logger.LogInformation("{Count} elements of type {ElementType} retrieved successfully.", elements.Count(), type);

        return elements.Select(e => new ElementModel(e.Name, e.Type, "Internal", e.Id));
    }
}
