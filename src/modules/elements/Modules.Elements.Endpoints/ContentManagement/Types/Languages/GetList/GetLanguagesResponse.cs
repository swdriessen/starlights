using Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.GetList;

public sealed record GetLanguagesResponse(IReadOnlyCollection<LanguageDataModel> Items);
