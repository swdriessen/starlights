namespace Starlights.Integration.Drivers.Elements.Extensions;

internal enum TestSpells
{
    Light,
    Fireball,
    MageArmor
}

internal static class ElementsDriverExtensions
{
    extension(ElementsCreationDriver driver)
    {
        public Task<Guid> CreateSpell(TestSpells spells)
        {
            return spells switch
            {
                TestSpells.Light => driver.CreateLightCantrip(),
                TestSpells.Fireball => driver.CreateFireballSpell(),
                TestSpells.MageArmor => driver.CreateMageArmorSpell(),
                _ => throw new ArgumentOutOfRangeException(nameof(spells), spells, null)
            };
        }

        public Task<Guid> CreateLightCantrip()
        {
            return driver.CreateSpellAsync(
                 name: "Light",
                 level: 0,
                 school: "Evocation",
                 time: "1 action",
                 range: "Touch",
                 duration: "1 hour",
                 isConcentration: false,
                 isRitual: false,
                 hasSomatic: true,
                 hasVerbal: true,
                 hasMaterial: true,
                 materialComponent: "A firefly or phosphorescent moss",
                 description: "You touch one object that is no larger than 10 feet in any dimension. Until the spell ends, the object glows with bright light in a 20-foot radius and dim light for an additional 20 feet."
             );
        }

        public Task<Guid> CreateFireballSpell()
        {
            return driver.CreateSpellAsync(
                name: "Fireball",
                level: 3,
                school: "Evocation",
                time: "1 action",
                range: "150 feet",
                duration: "Instantaneous",
                isConcentration: false,
                isRitual: false,
                hasSomatic: true,
                hasVerbal: true,
                hasMaterial: true,
                materialComponent: "A tiny ball of bat guano and sulfur",
                description: "A bright streak flashes from your pointing finger to a point you choose within range and then blossoms with a low roar into an explosion of flame."
            );
        }

        public Task<Guid> CreateMageArmorSpell()
        {
            return driver.CreateSpellAsync(
                name: "Mage Armor",
                level: 1,
                school: "Abjuration",
                time: "1 action",
                range: "Touch",
                duration: "8 hours",
                isConcentration: false,
                isRitual: false,
                hasSomatic: true,
                hasVerbal: true,
                hasMaterial: true,
                materialComponent: "A piece of cured leather",
                description: "You touch a willing creature who isn't wearing armor, and a protective magical force surrounds it until the spell ends."
            );
        }
    }

}
