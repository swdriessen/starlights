namespace Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.Update;

public sealed record UpdateSavingThrowRequest(Guid Id, string Name, Guid AbilityId, string Description);