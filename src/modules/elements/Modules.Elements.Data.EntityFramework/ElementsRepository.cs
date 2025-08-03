using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Elements.Data.EntityFramework;

internal class ElementsRepository : RepositoryBase<Element>, IElementsRepository
{
    private readonly ILogger<ElementsRepository> _logger;

    public ElementsRepository(ILogger<ElementsRepository> logger)
    {
        _logger = logger;
    }

    public async Task AddAsync(Element element)
    {
        using var _ = ElementsInstrumentation.StartActivity();

        _logger.LogInformation("add element '{ElementName}' with identifier {Identifier}", element.Name, element.Id.Value);
        await Entities.AddAsync(element);
    }

    public async Task<Element?> GetElementAsync(Guid identifier)
    {
        using var _ = ElementsInstrumentation.StartActivity();

        var element = await Entities.FirstOrDefaultAsync(e => e.Id == identifier);

        if (element is null)
        {
            _logger.LogWarning("element with identifier {Identifier} not found", identifier);
        }

        return element;
    }

    public async Task<List<Element>> GetElementsByTypeAsync(string type)
    {
        using var _ = ElementsInstrumentation.StartActivity();

        _logger.LogInformation("getting elements of type [{ElementType}]", type);

        return await Entities
            .Include(x => x.Components)
            .Where(element => element.Type == type)
            .ToListAsync();
    }

    public async Task<List<Element>> GetElementsByTypesAsync(IEnumerable<string> types)
    {
        using var _ = ElementsInstrumentation.StartActivity();

        _logger.LogInformation("getting elements of types [{ElementTypes}]", string.Join(", ", types));

        return await Entities
            .Include(x => x.Components)
            .Where(element => types.Contains(element.Type))
            .ToListAsync();
    }
}
