using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.Create;

public sealed record CreateSavingThrowRequest(string Name, Guid AbilityId);