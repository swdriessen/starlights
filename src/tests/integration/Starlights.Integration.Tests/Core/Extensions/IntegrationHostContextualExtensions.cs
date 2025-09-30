namespace Starlights.Integration.Tests.Core.Extensions;

public static class IntegrationHostContextualExtensions
{
    public const string CurrentCharacterIdentifierKey = "CURRENT-CHARACTER-ID";
    public const string CurrentCharacterNameKey = "CURRENT-CHARACTER-NAME";

    public static Guid GetCharacterIdentifier(this IntegrationHost host)
    {
        if (host.Properties.TryGetValue(CurrentCharacterIdentifierKey, out var value) && value is Guid characterId)
        {
            return characterId;
        }

        throw new InvalidOperationException($"Character identifier '{CurrentCharacterIdentifierKey}' not found in context properties. Ensure the character has been created and the context is initialized properly.");
    }

    public static void SetCharacterIdentifier(this IntegrationHost host, Guid characterId) => host.Properties[CurrentCharacterIdentifierKey] = characterId;
}