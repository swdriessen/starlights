namespace Starlights.Modules.Elements.Endpoints.Content.Labels.Update;

public sealed record UpdateElementLabelRequest(
    IReadOnlyCollection<string> Labels);
