namespace Starlights.Integration.Extensions;

/// <summary>
/// Provides extension methods for managing character-related context properties within an IntegrationHost instance.
/// </summary>
/// <remarks>
/// These extensions enable storing and retrieving character identifiers and names in the context
/// properties of an IntegrationHost. They are intended to simplify access to character-specific data during integration
/// scenarios.
/// </remarks>
public static class ScenarioContextExtensions
{
    public const string CurrentCharacterIdentifierKey = "CURRENT-CHARACTER-ID";

    [Obsolete("switch to characters context")]
    public static Guid GetCharacterIdentifier(this IIntegrationHost host)
    {
        return host.Properties.TryGetValue(CurrentCharacterIdentifierKey, out var value) && value is Guid characterId
            ? characterId
            : throw new InvalidOperationException($"Character identifier '{CurrentCharacterIdentifierKey}' not found in context properties. Ensure the character has been created and the context is initialized properly.");
    }

    [Obsolete("switch to characters context")]
    public static void SetCharacterIdentifier(this IIntegrationHost host, Guid characterId)
    {
        host.Properties[CurrentCharacterIdentifierKey] = characterId;
    }
}
