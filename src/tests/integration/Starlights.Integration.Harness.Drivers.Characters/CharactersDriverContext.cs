namespace Starlights.Integration.Drivers.Characters;

public sealed class CharactersDriverContext
{
    private readonly List<Character> _characters = [];

    public Character CurrentCharacter
    {
        get
        {
            if (_characters.Count == 0)
            {
                throw new InvalidOperationException("No characters have been created in the context.");
            }

            return _characters[^1];
        }
    }

    public void WithCharacter(Guid id, string name)
    {
        _characters.Add(new Character(id, name));
    }

    public record struct Character(Guid Id, string Name);
}