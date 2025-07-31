using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Extensions.Platform.Data.EntityFramework;
using Starlights.Modules.Elements.Domain;

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
        _logger.LogInformation("Adding new element with identifier {Identifier}.", element.Id);

        await Entities.AddAsync(element);
    }

    public async Task<Element?> GetElementAsync(Guid identifier)
    {
        var element = await Entities.FirstOrDefaultAsync(e => e.Id == identifier);

        if (element is null)
        {
            _logger.LogWarning("Element with identifier {Identifier} not found.", identifier);
        }
        else
        {
            _logger.LogInformation("Element with identifier {Identifier} retrieved successfully.", identifier);
        }

        return element;
    }

    public async Task<List<Element>> GetElementsByTypeAsync(string type)
    {
        _logger.LogInformation("Retrieving elements of type {Type}.", type);

        return await Entities
            .Include(x => x.Components)
            .Where(element => element.Type == type)
            .ToListAsync();
    }
}
