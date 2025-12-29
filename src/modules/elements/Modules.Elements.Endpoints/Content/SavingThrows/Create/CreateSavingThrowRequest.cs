using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Create;

public sealed record CreateSavingThrowRequest(string Name, Guid AbilityId);