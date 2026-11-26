namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A resolved or explicitly unavailable number with inspectable contributing origins.</summary>
public sealed record CharacterStatisticResponse(string Id, string Name, int? Value, IReadOnlyList<string> Sources);
