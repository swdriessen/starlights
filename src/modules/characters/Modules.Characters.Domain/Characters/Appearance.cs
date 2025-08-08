using System.Diagnostics;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Characters;

/// <summary>
/// Represents the appearance details of a character.
/// </summary>
[DebuggerDisplay("Id = {Id}, CharacterId = {CharacterId}")]
public sealed class Appearance : AggregateRoot<AppearanceId>
{
    private Appearance(CharacterId characterId, string? portraitUrl = null)
        : base(AppearanceId.New())
    {
        CharacterId = characterId;
        PortraitUrl = portraitUrl;
    }

    /// <summary>
    /// Gets the character ID that this appearance belongs to.
    /// </summary>
    public CharacterId CharacterId { get; private set; }

    /// <summary>
    /// Gets the portrait URL for this appearance.
    /// </summary>
    public string? PortraitUrl { get; private set; }

    /// <summary>
    /// Updates the portrait for the character appearance.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the portrait URL is null, empty, or whitespace.</exception>
    public void UpdatePortraitUrl(string portraitUrl)
    {
        if (string.IsNullOrWhiteSpace(portraitUrl))
        {
            throw new ArgumentException("The portrait url cannot be null, empty, or whitespace.", nameof(portraitUrl));
        }

        PortraitUrl = portraitUrl;
    }

    /// <summary>
    /// Removes the portrait by setting the portrait URL to null.
    /// </summary>
    public void RemovePortrait()
    {
        PortraitUrl = null;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Appearance"/> class for the specified character.
    /// </summary>
    public static Appearance Create(CharacterId characterId)
    {
        return new Appearance(characterId);
    }
}