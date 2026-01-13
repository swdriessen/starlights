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

    public void Add(Element element)
    {
        //_logger.LogInformation("add element '{ElementName}' with identifier {Identifier}", element.Name, element.Id.Value);
        Entities.Add(element);
    }

    public async Task<bool> DeleteElementAsync(Guid identifier)
    {
        var element = await Entities.SingleOrDefaultAsync(e => e.Id == identifier);
        if (element is null)
        {
            _logger.LogWarning("element with identifier {Identifier} not found", identifier);
            return false;
        }

        Entities.Remove(element);
        return true;
    }

    public async Task<Element?> GetElementAsync(Guid identifier)
    {
        var element = await Entities
            .Include(x => x.Components.OrderBy(c => c.OrderSequence))
            .SingleOrDefaultAsync(e => e.Id == identifier);

        if (element is null)
        {
            _logger.LogWarning("element with identifier {Identifier} not found", identifier);
        }

        return element;
    }

    public async Task<List<Element>> GetElementsAsync()
    {
        _logger.LogInformation("getting all elements");

        return await Entities
            .Include(x => x.Components.OrderBy(c => c.OrderSequence))
            .ToListAsync();
    }

    public async Task<List<Element>> GetElementsByTypeAsync(string type)
    {
        _logger.LogInformation("getting elements of type [{ElementType}]", type);

        return await Entities
            .Include(x => x.Components.OrderBy(c => c.OrderSequence))
            .Where(element => element.Type == type)
            .ToListAsync();
    }

    public async Task<List<Element>> GetElementsByTypesAsync(IEnumerable<string> types)
    {
        _logger.LogInformation("getting elements of types [{ElementTypes}]", string.Join(", ", types));

        return await Entities
            .Include(x => x.Components.OrderBy(c => c.OrderSequence))
            .Where(element => types.Contains(element.Type))
            .ToListAsync();
    }
}
