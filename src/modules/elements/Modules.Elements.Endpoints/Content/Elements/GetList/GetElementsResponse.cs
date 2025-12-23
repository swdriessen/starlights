using Starlights.Modules.Elements.Endpoints.Content.Elements;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.GetList;

public record GetElementsResponse(IReadOnlyCollection<ElementDataModel> Items);
