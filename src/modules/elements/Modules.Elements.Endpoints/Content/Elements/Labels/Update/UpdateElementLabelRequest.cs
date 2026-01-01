namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Update;

public sealed record UpdateElementLabelRequest(
    IReadOnlyCollection<string> Labels);
