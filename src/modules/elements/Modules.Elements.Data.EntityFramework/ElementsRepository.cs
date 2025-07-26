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

    public async Task<Element?> GetElementAsync(Guid identifier)
    {
        var element = await Entities.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == identifier);

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

    public async Task<IEnumerable<Element>> GetElementsByTypeAsync(string type)
    {
        return await Entities.AsNoTracking()
            .Where(element => element.Type == type)
            .ToListAsync();
    }
}
