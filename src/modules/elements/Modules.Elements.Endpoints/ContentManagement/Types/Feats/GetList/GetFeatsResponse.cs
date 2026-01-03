using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.GetList;

public sealed record GetFeatsResponse(IReadOnlyCollection<FeatDataModel> Items);
