/** Mock data for the builder mockup (v5). Descriptions are paraphrased placeholders, not shipped content. */

export type Ab = "STR" | "DEX" | "CON" | "INT" | "WIS" | "CHA"

export const abilityList: { key: Ab; name: string; blurb: string }[] = [
  {
    key: "STR",
    name: "Strength",
    blurb: "Raw physical power: melee attacks, Athletics, carrying.",
  },
  {
    key: "DEX",
    name: "Dexterity",
    blurb: "Agility and reflexes: initiative, ranged attacks, light armour.",
  },
  {
    key: "CON",
    name: "Constitution",
    blurb: "Health and stamina: hit points and endurance.",
  },
  {
    key: "INT",
    name: "Intelligence",
    blurb: "Reasoning and memory: lore skills.",
  },
  {
    key: "WIS",
    name: "Wisdom",
    blurb: "Awareness and insight: Perception, Insight, Survival.",
  },
  {
    key: "CHA",
    name: "Charisma",
    blurb: "Force of personality: social skills.",
  },
]

export const skillList: { name: string; ab: Ab }[] = [
  { name: "Acrobatics", ab: "DEX" },
  { name: "Animal Handling", ab: "WIS" },
  { name: "Arcana", ab: "INT" },
  { name: "Athletics", ab: "STR" },
  { name: "Deception", ab: "CHA" },
  { name: "History", ab: "INT" },
  { name: "Insight", ab: "WIS" },
  { name: "Intimidation", ab: "CHA" },
  { name: "Investigation", ab: "INT" },
  { name: "Medicine", ab: "WIS" },
  { name: "Nature", ab: "INT" },
  { name: "Perception", ab: "WIS" },
  { name: "Performance", ab: "CHA" },
  { name: "Persuasion", ab: "CHA" },
  { name: "Religion", ab: "INT" },
  { name: "Sleight of Hand", ab: "DEX" },
  { name: "Stealth", ab: "DEX" },
  { name: "Survival", ab: "WIS" },
]

export type Feat = { name: string; text: string }
export const feats: Record<string, Feat> = {
  Alert: {
    name: "Alert",
    text: "You are hard to catch off guard. Add your proficiency bonus to your initiative rolls, and you can swap initiative with a willing ally.",
  },
  "Savage Attacker": {
    name: "Savage Attacker",
    text: "Once per turn, when you hit with a weapon, you can roll the weapon's damage dice twice and use either result.",
  },
  "Magic Initiate (Cleric)": {
    name: "Magic Initiate (Cleric)",
    text: "You learn two cleric cantrips and one first-level cleric spell that you can cast once per long rest.",
  },
  "Magic Initiate (Wizard)": {
    name: "Magic Initiate (Wizard)",
    text: "You learn two wizard cantrips and one first-level wizard spell that you can cast once per long rest.",
  },
  Skilled: {
    name: "Skilled",
    text: "You gain proficiency in any combination of three skills or tools of your choice.",
  },
}
export const originFeatNames = Object.keys(feats)

export type Trait = { name: string; text: string }
export type SpeciesChoice = {
  id: string
  label: string
  kind: "skill" | "feat"
  options: string[]
}
export type Species = {
  id: string
  name: string
  blurb: string
  size: string
  speed: number
  hpPerLevel?: number
  traits: Trait[]
  choices: SpeciesChoice[]
  about: string[]
}

export const speciesList: Species[] = [
  {
    id: "human",
    name: "Human",
    blurb: "Versatile and driven, at home in every corner of the world.",
    size: "Medium",
    speed: 30,
    traits: [
      {
        name: "Resourceful",
        text: "You gain Heroic Inspiration whenever you finish a long rest.",
      },
      {
        name: "Skillful",
        text: "You gain proficiency in one skill of your choice.",
      },
      { name: "Versatile", text: "You gain an Origin feat of your choice." },
    ],
    choices: [
      {
        id: "skillful",
        label: "Skillful: a skill",
        kind: "skill",
        options: skillList.map((s) => s.name),
      },
      {
        id: "versatile",
        label: "Versatile: an Origin feat",
        kind: "feat",
        options: originFeatNames,
      },
    ],
    about: [
      "Humans are the most widespread folk of the known lands, and the most varied. Their short lives push them to build, travel and argue fast, so almost every trade and calling has a human practitioner.",
      "In play, the human's strength is flexibility: a free skill, a free Origin feat, and a steady supply of Heroic Inspiration.",
    ],
  },
  {
    id: "elf",
    name: "Elf",
    blurb: "Long-lived, graceful and attuned to the fey places of the world.",
    size: "Medium",
    speed: 30,
    traits: [
      {
        name: "Darkvision",
        text: "You can see in dim light within 60 feet as if it were bright, and in darkness as if it were dim.",
      },
      {
        name: "Fey Ancestry",
        text: "You have advantage on saving throws to avoid or end the charmed condition.",
      },
      {
        name: "Keen Senses",
        text: "You gain proficiency in Insight, Perception or Survival.",
      },
      {
        name: "Trance",
        text: "You rest in a light meditation instead of sleeping, finishing a long rest in four hours.",
      },
    ],
    choices: [
      {
        id: "keen",
        label: "Keen Senses: a skill",
        kind: "skill",
        options: ["Insight", "Perception", "Survival"],
      },
    ],
    about: [
      "Elves live for centuries and keep an easy, watchful relationship with time. Many carry a quiet bond to the fey, and their senses are sharper than most.",
      "Mechanically, elves trade the human's flexibility for senses, resistance to charm, and four-hour rests.",
    ],
  },
  {
    id: "dwarf",
    name: "Dwarf",
    blurb: "Stout, stubborn and tireless, shaped by stone and craft.",
    size: "Medium",
    speed: 30,
    hpPerLevel: 1,
    traits: [
      {
        name: "Darkvision",
        text: "You can see in dim light within 120 feet as if it were bright.",
      },
      {
        name: "Dwarven Resilience",
        text: "You have resistance to poison damage and advantage on saves against being poisoned.",
      },
      {
        name: "Dwarven Toughness",
        text: "Your hit point maximum increases by 1, and by 1 again whenever you gain a level.",
      },
    ],
    choices: [],
    about: [
      "Dwarves are patient builders and unforgiving grudge-keepers. Clan, craft and a good fire count for more than most things.",
      "They are among the hardiest folk to adventure: extra hit points every level and strong resistance to poison.",
    ],
  },
  {
    id: "halfling",
    name: "Halfling",
    blurb: "Small, brave and lucky, with a talent for staying out of trouble.",
    size: "Small",
    speed: 30,
    traits: [
      {
        name: "Brave",
        text: "You have advantage on saving throws to avoid or end the frightened condition.",
      },
      {
        name: "Halfling Nimbleness",
        text: "You can move through the space of any creature larger than you.",
      },
      {
        name: "Luck",
        text: "When you roll a 1 on a d20 test, you can reroll it and must use the new roll.",
      },
    ],
    choices: [],
    about: [
      "Halflings favour home, hearth and a good meal, and are unnervingly calm when things go wrong. Luck follows them more than reason says it should.",
    ],
  },
  {
    id: "dragonborn",
    name: "Dragonborn",
    blurb: "Scaled and proud, carrying the breath of a draconic ancestor.",
    size: "Medium",
    speed: 30,
    traits: [
      {
        name: "Draconic Ancestry",
        text: "Choose a dragon type; it sets your breath weapon and damage resistance.",
      },
      {
        name: "Breath Weapon",
        text: "You exhale destructive energy in a cone, a number of times equal to your proficiency bonus per long rest.",
      },
    ],
    choices: [],
    about: [
      "Dragonborn trace their line to dragons and wear that heritage openly, with honour as a point of pride.",
    ],
  },
  {
    id: "gnome",
    name: "Gnome",
    blurb: "Curious tinkerers with a sharp mind and a stubborn will.",
    size: "Small",
    speed: 30,
    traits: [
      {
        name: "Darkvision",
        text: "You can see in dim light within 60 feet as if it were bright.",
      },
      {
        name: "Gnomish Cunning",
        text: "You have advantage on Intelligence, Wisdom and Charisma saving throws.",
      },
    ],
    choices: [],
    about: [
      "Gnomes are inventors and eccentrics, quick to wonder and slow to give up on a puzzle.",
    ],
  },
  {
    id: "goliath",
    name: "Goliath",
    blurb: "Towering mountain-folk who measure themselves against the peaks.",
    size: "Medium",
    speed: 35,
    traits: [
      {
        name: "Giant Ancestry",
        text: "Choose a giant boon, usable a number of times equal to your proficiency bonus.",
      },
      {
        name: "Powerful Build",
        text: "You count as one size larger when determining carrying capacity.",
      },
    ],
    choices: [],
    about: [
      "Goliaths live high and hard, and prize fair play and personal accomplishment above comfort.",
    ],
  },
  {
    id: "orc",
    name: "Orc",
    blurb: "Fierce and enduring, with a surge of strength when pressed.",
    size: "Medium",
    speed: 30,
    traits: [
      {
        name: "Adrenaline Rush",
        text: "You can take the Dash action as a bonus action and gain temporary hit points when you do.",
      },
      {
        name: "Relentless Endurance",
        text: "Once per long rest, when reduced to 0 hit points you can drop to 1 instead.",
      },
    ],
    choices: [],
    about: [
      "Orcs are built for endurance and sudden violence, and are loyal to those who earn it.",
    ],
  },
  {
    id: "tiefling",
    name: "Tiefling",
    blurb: "Marked by a fiendish legacy and a defiant sense of self.",
    size: "Medium",
    speed: 30,
    traits: [
      {
        name: "Fiendish Legacy",
        text: "Choose a legacy that grants resistance and innate spellcasting.",
      },
      {
        name: "Otherworldly Presence",
        text: "You know the Thaumaturgy cantrip.",
      },
    ],
    choices: [],
    about: [
      "Tieflings carry a trace of the lower planes and are judged for it more often than they deserve.",
    ],
  },
]

export type Background = {
  id: string
  name: string
  blurb: string
  allowed: Ab[]
  skills: [string, string]
  tool: string
  feat: string
  equipA: { items: string[]; gp: number }
  equipBGp: number
  about: string[]
}

export const backgroundList: Background[] = [
  {
    id: "soldier",
    name: "Soldier",
    blurb:
      "You trained for war and learned discipline, drills and how to read a fight.",
    allowed: ["STR", "DEX", "CON"],
    skills: ["Athletics", "Intimidation"],
    tool: "Gaming set (dice)",
    feat: "Savage Attacker",
    equipA: {
      items: [
        "Spear",
        "Shortbow",
        "20 Arrows",
        "Quiver",
        "Gaming set (dice)",
        "Healer's Kit",
        "Travel Clothes",
      ],
      gp: 14,
    },
    equipBGp: 50,
    about: [
      "A soldier has stood in a line, kept watch through cold nights and followed orders that were not always wise. Whether in a standing army, a militia or a mercenary band, the habits of the barracks stay.",
      "This background favours the direct approach: Strength, Dexterity or Constitution, proficiency in Athletics and Intimidation, and the Savage Attacker feat.",
    ],
  },
  {
    id: "acolyte",
    name: "Acolyte",
    blurb:
      "You served in a temple and learned rites, scripture and the weight of faith.",
    allowed: ["INT", "WIS", "CHA"],
    skills: ["Insight", "Religion"],
    tool: "Calligrapher's Supplies",
    feat: "Magic Initiate (Cleric)",
    equipA: {
      items: [
        "Calligrapher's Supplies",
        "Book (prayers)",
        "Holy Symbol",
        "Parchment (10 sheets)",
        "Robe",
      ],
      gp: 8,
    },
    equipBGp: 50,
    about: [
      "An acolyte spent formative years in service to a temple, learning its rituals and its politics. You know how a congregation thinks and where its secrets are kept.",
    ],
  },
  {
    id: "criminal",
    name: "Criminal",
    blurb:
      "You lived by your wits and your fingers, and know every back door in town.",
    allowed: ["DEX", "CON", "INT"],
    skills: ["Sleight of Hand", "Stealth"],
    tool: "Thieves' Tools",
    feat: "Alert",
    equipA: {
      items: [
        "Dagger (2)",
        "Thieves' Tools",
        "Crowbar",
        "Pouch (2)",
        "Travel Clothes",
      ],
      gp: 16,
    },
    equipBGp: 50,
    about: [
      "A criminal knows how to move unseen, how to read a mark and when to leave. Whether a street thief or a fence, you have contacts in the places the law avoids.",
    ],
  },
  {
    id: "sage",
    name: "Sage",
    blurb: "You spent years among books, scholars and unanswered questions.",
    allowed: ["CON", "INT", "WIS"],
    skills: ["Arcana", "History"],
    tool: "Calligrapher's Supplies",
    feat: "Magic Initiate (Wizard)",
    equipA: {
      items: [
        "Quarterstaff",
        "Calligrapher's Supplies",
        "Book (history)",
        "Parchment (8 sheets)",
        "Robe",
      ],
      gp: 8,
    },
    equipBGp: 50,
    about: [
      "A sage is a researcher and a collector of facts. You know where the answers usually are, and who to ask when they are not written down.",
    ],
  },
]

export const fighter = {
  id: "fighter",
  name: "Fighter",
  hitDie: 10,
  skillOptions: [
    "Acrobatics",
    "Animal Handling",
    "Athletics",
    "History",
    "Insight",
    "Intimidation",
    "Perception",
    "Persuasion",
    "Survival",
  ],
  styles: [
    {
      name: "Archery",
      text: "You gain a +2 bonus to attack rolls you make with ranged weapons.",
    },
    {
      name: "Defense",
      text: "While you are wearing armor, you gain a +1 bonus to Armor Class.",
    },
    {
      name: "Great Weapon Fighting",
      text: "When you roll a 1 or 2 on a damage die with a two-handed melee weapon, you can treat it as a 3.",
    },
    {
      name: "Two-Weapon Fighting",
      text: "You can add your ability modifier to the damage of your off-hand attack.",
    },
  ],
  masteries: [
    {
      name: "Greatsword",
      mastery: "Graze",
      text: "2d6 slashing, heavy, two-handed. Graze deals damage equal to your ability modifier on a miss.",
    },
    {
      name: "Flail",
      mastery: "Sap",
      text: "1d8 bludgeoning. Sap gives the target disadvantage on its next attack.",
    },
    {
      name: "Javelin",
      mastery: "Slow",
      text: "1d6 piercing, thrown. Slow reduces the target's speed by 10 feet.",
    },
    {
      name: "Longsword",
      mastery: "Sap",
      text: "1d8 slashing, versatile. Sap gives the target disadvantage on its next attack.",
    },
    {
      name: "Rapier",
      mastery: "Vex",
      text: "1d8 piercing, finesse. Vex gives you advantage on your next attack against the target.",
    },
    {
      name: "Battleaxe",
      mastery: "Topple",
      text: "1d8 slashing, versatile. Topple can knock the target prone.",
    },
    {
      name: "Greataxe",
      mastery: "Cleave",
      text: "1d12 slashing, heavy, two-handed. Cleave lets you strike a second nearby target.",
    },
    {
      name: "Handaxe",
      mastery: "Vex",
      text: "1d6 slashing, light, thrown. Vex gives you advantage on your next attack.",
    },
  ],
  fighterEquipA: [
    "Chain Mail",
    "Greatsword",
    "Flail",
    "Javelin (8)",
    "Dungeoneer's Pack",
  ],
  fighterEquipAGp: 4,
  fighterEquipBGp: 155,
  about: [
    "Fighters master weapons and armour through relentless training. They are the most dependable martial class: durable, flexible and able to stand in the front line or on the back foot.",
    "At level 1 a Fighter chooses a Fighting Style, learns Second Wind (a bonus-action self-heal) and gains Weapon Mastery with three weapon types.",
  ],
}

export const classRoster: { name: string; role: string; ready: boolean }[] = [
  { name: "Fighter", role: "Martial · d10", ready: true },
  { name: "Barbarian", role: "Martial · d12", ready: false },
  { name: "Paladin", role: "Martial · d10", ready: false },
  { name: "Ranger", role: "Martial · d10", ready: false },
  { name: "Rogue", role: "Martial · d8", ready: false },
  { name: "Monk", role: "Martial · d8", ready: false },
  { name: "Cleric", role: "Divine · d8", ready: false },
  { name: "Druid", role: "Primal · d8", ready: false },
  { name: "Bard", role: "Arcane · d8", ready: false },
  { name: "Sorcerer", role: "Arcane · d6", ready: false },
  { name: "Warlock", role: "Arcane · d8", ready: false },
  { name: "Wizard", role: "Arcane · d6", ready: false },
]

export const languageOptions = [
  "Common Sign Language",
  "Dwarvish",
  "Elvish",
  "Giant",
  "Gnomish",
  "Goblin",
  "Halfling",
  "Orc",
]

export const pointCost: Record<number, number> = {
  8: 0,
  9: 1,
  10: 2,
  11: 3,
  12: 4,
  13: 5,
  14: 7,
  15: 9,
}
export const standardArray = [15, 14, 13, 12, 10, 8]
export const rolledPool = [17, 14, 13, 12, 10, 8]

export const portraits = [14, 6, 12, 15, 8, 10, 16, 4].map(
  (n) => `/images/portraits/p${n}.webp`
)

export const stages = [
  { id: "concept", label: "Concept", hint: "Name, portrait, notes" },
  { id: "origin", label: "Origin", hint: "Species and background" },
  { id: "class", label: "Class", hint: "Skills, style, masteries" },
  { id: "abilities", label: "Abilities", hint: "Score generation" },
  { id: "equipment", label: "Equipment", hint: "Packages and gear" },
  { id: "review", label: "Review", hint: "Check and finish" },
] as const
export type StageId = (typeof stages)[number]["id"]
