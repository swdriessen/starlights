using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.Create;

public sealed record CreateSavingThrowRequest(string Name, Guid AbilityId);