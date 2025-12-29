namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.GetSavingThrows;

public sealed record GetSavingThrowsResponse(IReadOnlyList<SavingThrowListItem> SavingThrows);