namespace Starlights.Modules.Elements.Endpoints.Content.Feats.GetList;

public sealed record GetFeatsResponse(IReadOnlyCollection<FeatDataModel> Items);
