namespace Starlights.Modules.Elements.Endpoints.Content.Labels.Create;

public sealed record CreateElementLabelRequest(
    IReadOnlyCollection<string> Labels);
