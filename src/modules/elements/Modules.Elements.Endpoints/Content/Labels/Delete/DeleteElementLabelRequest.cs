namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Delete;

public sealed record DeleteElementLabelRequest(
    IReadOnlyCollection<string> Labels);
