namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Update;

public sealed record UpdateSavingThrowRequest(Guid Id, string Name, Guid AbilityId, string Description);