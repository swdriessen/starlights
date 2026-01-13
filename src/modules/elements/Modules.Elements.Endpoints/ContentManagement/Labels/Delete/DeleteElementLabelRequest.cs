namespace Starlights.Modules.Elements.Endpoints.Content.Labels.Delete;

public sealed record DeleteElementLabelRequest(
    IReadOnlyCollection<string> Labels);
