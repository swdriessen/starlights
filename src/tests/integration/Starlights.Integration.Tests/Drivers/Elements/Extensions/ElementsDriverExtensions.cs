namespace Starlights.Integration.Drivers.Elements.Extensions;

public enum TestSpells
{
    Light,
    Fireball,
    MageArmor
}

public static class SpellDefinitions
{
    public static CreateSpellProperties Light { get; } = new()
    {
        Name = "Light",
        Level = 0,
        MagicSchool = "Evocation",
        CastingTime = "1 action",
        Range = "Touch",
        Duration = "1 hour",
        IsConcentration = false,
        IsRitual = false,
        HasSomatic = true,
        HasVerbal = true,
        HasMaterial = true,
        MaterialComponent = "A firefly or phosphorescent moss",
        Description = "You touch one object that is no larger than 10 feet in any dimension. Until the spell ends, the object glows with bright light in a 20-foot radius and dim light for an additional 20 feet."
    };

    public static CreateSpellProperties Fireball { get; } = new()
    {
        Name = "Fireball",
        Level = 3,
        MagicSchool = "Evocation",
        CastingTime = "1 action",
        Range = "150 feet",
        Duration = "Instantaneous",
        IsConcentration = false,
        IsRitual = false,
        HasSomatic = true,
        HasVerbal = true,
        HasMaterial = true,
        MaterialComponent = "A tiny ball of bat guano and sulfur",
        Description = "A bright streak flashes from your pointing finger to a point you choose within range and then blossoms with a low roar into an explosion of flame."
    };

    public static CreateSpellProperties MageArmor { get; } = new()
    {
        Name = "Mage Armor",
        Level = 1,
        MagicSchool = "Abjuration",
        CastingTime = "1 action",
        Range = "Touch",
        Duration = "8 hours",
        IsConcentration = false,
        IsRitual = false,
        HasSomatic = true,
        HasVerbal = true,
        HasMaterial = true,
        MaterialComponent = "A piece of cured leather",
        Description = "You touch a willing creature who isn't wearing armor, and a protective magical force surrounds it until the spell ends."
    };
}

public static class ElementsDriverExtensions
{
    extension(ElementsCreationDriver driver)
    {
        public Task<Guid> CreateSpell(TestSpells spells)
        {
            var definition = spells switch
            {
                TestSpells.Light => SpellDefinitions.Light,
                TestSpells.Fireball => SpellDefinitions.Fireball,
                TestSpells.MageArmor => SpellDefinitions.MageArmor,
                _ => throw new ArgumentOutOfRangeException(nameof(spells), spells, null)
            };

            return driver.CreateSpellAsync(definition);
        }

        public Task<Guid> CreateLightCantrip()
        {
            return driver.CreateSpellAsync(SpellDefinitions.Light);
        }

        public Task<Guid> CreateFireballSpell()
        {
            return driver.CreateSpellAsync(SpellDefinitions.Fireball);
        }

        public Task<Guid> CreateMageArmorSpell()
        {
            return driver.CreateSpellAsync(SpellDefinitions.MageArmor);
        }
    }
}
