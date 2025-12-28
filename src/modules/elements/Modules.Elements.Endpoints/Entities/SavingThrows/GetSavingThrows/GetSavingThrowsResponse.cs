namespace Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.GetSavingThrows;

public sealed record GetSavingThrowsResponse(IReadOnlyList<SavingThrowListItem> SavingThrows);