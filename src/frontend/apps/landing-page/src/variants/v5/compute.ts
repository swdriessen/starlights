import {
  abilityList,
  backgroundList,
  fighter,
  pointCost,
  skillList,
  speciesList,
  type Ab,
  type StageId,
} from "./data"

export type Method = "standard" | "pointbuy" | "roll"
export type Scores = Record<Ab, number>

export type BuildState = {
  name: string
  portrait: number
  notes: string
  speciesId: string
  backgroundId: string
  /** Recorded species choices, keyed `${speciesId}:${choiceId}`. Kept when the species changes. */
  picks: Record<string, string>
  /** Recorded background ability increases, keyed by background id. Kept when the background changes. */
  bonus: Record<string, { two?: Ab; one?: Ab }>
  languages: string[]
  fighterSkills: string[]
  style: string
  masteries: string[]
  method: Method
  standard: Scores
  roll: Scores
  pointbuy: Scores
  fighterPackage: "A" | "B"
  backgroundPackage: "A" | "B"
  claimed: { fighter: boolean; background: boolean }
  worn: boolean
  wielded: boolean
}

const keys = abilityList.map((a) => a.key)
const fromArray = (values: number[]): Scores =>
  Object.fromEntries(keys.map((k, i) => [k, values[i]])) as Scores

/** The first checkpoint's reference build: Human Fighter with the Soldier background. */
export const initialState: BuildState = {
  name: "Bryn Holloway",
  portrait: 0,
  notes:
    "Former shield-wall sergeant. Left the legion after a campaign that went wrong, and is looking for work that lets her sleep.",
  speciesId: "human",
  backgroundId: "soldier",
  picks: { "human:skillful": "Perception", "human:versatile": "Alert" },
  bonus: { soldier: { two: "STR", one: "CON" } },
  languages: ["Dwarvish", "Elvish"],
  fighterSkills: ["Insight", "Survival"],
  style: "Defense",
  masteries: ["Greatsword", "Flail", "Javelin"],
  method: "standard",
  // STR 15, DEX 14, CON 13, INT 8, WIS 10, CHA 12: the reference build's standard-array assignment.
  standard: fromArray([15, 14, 13, 8, 10, 12]),
  // Recorded 4d6-drop-lowest results, assigned (the pool is 17, 14, 13, 12, 10, 8).
  roll: fromArray([17, 14, 13, 8, 10, 12]),
  // Exactly 27 points: 9 + 7 + 5 + 0 + 2 + 4.
  pointbuy: fromArray([15, 14, 13, 8, 10, 12]),
  fighterPackage: "A",
  backgroundPackage: "A",
  claimed: { fighter: true, background: true },
  worn: true,
  wielded: true,
}

export const mod = (score: number) => Math.floor((score - 10) / 2)
export const fmt = (n: number) => (n >= 0 ? `+${n}` : `\u2212${Math.abs(n)}`)

export type Issue = { stage: StageId; text: string }
export type StageStatus = {
  state: "complete" | "incomplete" | "optional" | "ready" | "blocked"
  detail: string
}

export function pointBuySpent(scores: Scores) {
  return keys.reduce((sum, k) => sum + (pointCost[scores[k]] ?? 0), 0)
}

export function derive(s: BuildState) {
  const species = speciesList.find((x) => x.id === s.speciesId)!
  const bg = backgroundList.find((x) => x.id === s.backgroundId)!
  const prof = 2

  const base: Scores =
    s.method === "standard"
      ? s.standard
      : s.method === "roll"
        ? s.roll
        : s.pointbuy
  const bonusNow = s.bonus[bg.id] ?? {}
  const abilities = Object.fromEntries(
    keys.map((k) => {
      const inc = (bonusNow.two === k ? 2 : 0) + (bonusNow.one === k ? 1 : 0)
      const final = Math.min(20, base[k] + inc)
      return [k, { base: base[k], inc, final, mod: mod(final) }]
    })
  ) as Record<Ab, { base: number; inc: number; final: number; mod: number }>

  // Choices that apply now vs. recorded-but-dormant ones.
  const activePicks = species.choices.map((c) => ({
    ...c,
    value: s.picks[`${species.id}:${c.id}`] as string | undefined,
  }))
  const dormant: { label: string; value: string; reason: string }[] = []
  for (const sp of speciesList) {
    if (sp.id === species.id) continue
    for (const c of sp.choices) {
      const v = s.picks[`${sp.id}:${c.id}`]
      if (v)
        dormant.push({
          label: `${sp.name}: ${c.label.split(":")[0]}`,
          value: v,
          reason: `Species is ${species.name}`,
        })
    }
  }
  for (const b of backgroundList) {
    if (b.id === bg.id) continue
    const bn = s.bonus[b.id]
    if (bn?.two || bn?.one) {
      dormant.push({
        label: `${b.name}: ability increases`,
        value: [bn.two && `+2 ${bn.two}`, bn.one && `+1 ${bn.one}`]
          .filter(Boolean)
          .join(", "),
        reason: `Background is ${bg.name}`,
      })
    }
  }

  // Skill proficiencies and where they came from.
  const skillSources: Record<string, string[]> = {}
  const addSkill = (skill: string | undefined, source: string) => {
    if (!skill) return
    ;(skillSources[skill] ??= []).push(source)
  }
  s.fighterSkills.forEach((k) => addSkill(k, "Fighter"))
  activePicks
    .filter((c) => c.kind === "skill")
    .forEach((c) => addSkill(c.value, species.name))
  bg.skills.forEach((k) => addSkill(k, bg.name))

  // Feats and duplicates.
  const featSources: { name: string; source: string }[] = []
  activePicks
    .filter((c) => c.kind === "feat" && c.value)
    .forEach((c) =>
      featSources.push({
        name: c.value!,
        source: `${species.name}: ${c.label.split(":")[0]}`,
      })
    )
  featSources.push({ name: bg.feat, source: bg.name })
  if (s.style)
    featSources.push({ name: `Fighting Style: ${s.style}`, source: "Fighter" })
  const hasFeat = (n: string) => featSources.some((f) => f.name === n)

  const issues: Issue[] = []
  for (const [skill, src] of Object.entries(skillSources)) {
    if (src.length > 1) {
      const fighterInvolved = src.includes("Fighter")
      issues.push({
        stage: fighterInvolved ? "class" : "origin",
        text: `${skill} is granted by ${src.join(" and ")}. Choose a different skill for one of them.`,
      })
    }
  }
  const origin = featSources.filter((f) => !f.name.startsWith("Fighting Style"))
  for (const f of origin) {
    if (
      origin.filter((o) => o.name === f.name).length > 1 &&
      !issues.some((i) => i.text.startsWith(`${f.name} is`))
    ) {
      issues.push({
        stage: "origin",
        text: `${f.name} is granted twice (${origin
          .filter((o) => o.name === f.name)
          .map((o) => o.source)
          .join(" and ")}). Choose a different feat.`,
      })
    }
  }

  // Equipment, claimed separately from the package choice.
  const inventory: { name: string; qty: number; kind?: "armor" | "weapon" }[] =
    []
  let gp = 0
  if (s.claimed.fighter) {
    if (s.fighterPackage === "A") {
      fighter.fighterEquipA.forEach((n) => {
        const m = n.match(/^(.*) \((\d+)\)$/)
        inventory.push({
          name: m ? m[1] : n,
          qty: m ? Number(m[2]) : 1,
          kind:
            n === "Chain Mail"
              ? "armor"
              : n === "Greatsword"
                ? "weapon"
                : undefined,
        })
      })
      gp += fighter.fighterEquipAGp
    } else gp += fighter.fighterEquipBGp
  }
  if (s.claimed.background) {
    if (s.backgroundPackage === "A") {
      bg.equipA.items.forEach((n) => inventory.push({ name: n, qty: 1 }))
      gp += bg.equipA.gp
    } else gp += bg.equipBGp
  }
  const hasChain = inventory.some((i) => i.kind === "armor")
  const hasSword = inventory.some((i) => i.kind === "weapon")
  const wearing = s.worn && hasChain
  const wielding = s.wielded && hasSword

  // Armour class, hit points, initiative, speed.
  const dex = abilities.DEX.mod
  const defense = s.style === "Defense"
  const acParts: string[] = []
  let ac: number
  if (wearing) {
    ac = 16
    acParts.push("Chain mail 16")
    if (defense) {
      ac += 1
      acParts.push("Defense +1")
    }
  } else {
    ac = 10 + dex
    acParts.push("10", `DEX ${fmt(dex)}`)
  }

  const conMod = abilities.CON.mod
  const hpParts = [`${fighter.hitDie}`, `CON ${fmt(conMod)}`]
  let hp = fighter.hitDie + conMod
  if (species.hpPerLevel) {
    hp += species.hpPerLevel
    hpParts.push(`${species.name} +${species.hpPerLevel}`)
  }

  const alert = hasFeat("Alert")
  const initiative = dex + (alert ? prof : 0)
  const initParts = [
    `DEX ${fmt(dex)}`,
    ...(alert ? [`Alert ${fmt(prof)}`] : []),
  ]

  let speed = species.speed
  const speedNotes: string[] = []
  if (wearing && abilities.STR.final < 13) {
    speed -= 10
    speedNotes.push("Chain mail needs STR 13: \u221210 ft")
  }

  // Saving throws and skills.
  const saveProf: Record<Ab, boolean> = {
    STR: true,
    CON: true,
    DEX: false,
    INT: false,
    WIS: false,
    CHA: false,
  }
  const saves = keys.map((k) => ({
    key: k,
    prof: saveProf[k],
    value: abilities[k].mod + (saveProf[k] ? prof : 0),
  }))
  const skills = skillList.map((sk) => {
    const proficient = !!skillSources[sk.name]
    return {
      ...sk,
      proficient,
      value: abilities[sk.ab].mod + (proficient ? prof : 0),
    }
  })
  const perception = skills.find((x) => x.name === "Perception")!.value

  const strMod = abilities.STR.mod
  const attack = wielding
    ? {
        name: "Greatsword",
        hit: strMod + prof,
        damage: `2d6 ${strMod >= 0 ? "+" : "\u2212"} ${Math.abs(strMod)} slashing`,
        mastery: s.masteries.includes("Greatsword") ? "Graze" : "\u2014",
      }
    : undefined

  const spent = pointBuySpent(s.pointbuy)
  const method = { spent, remaining: 27 - spent }

  // Stage completion.
  const originMissing: string[] = []
  const bn = bonusNow
  if (!bn.two) originMissing.push("a +2 ability increase")
  if (!bn.one) originMissing.push("a +1 ability increase")
  activePicks.forEach((c) => {
    if (!c.value) originMissing.push(c.label.split(":")[0])
  })
  if (s.languages.length < 2) originMissing.push("two languages")
  const originIssues = issues.filter((i) => i.stage === "origin")
  const classIssues = issues.filter((i) => i.stage === "class")
  const classMissing: string[] = []
  if (s.fighterSkills.length < 2) classMissing.push("two skills")
  if (!s.style) classMissing.push("a Fighting Style")
  if (s.masteries.length < 3) classMissing.push("three weapon masteries")

  const unclaimed = (s.claimed.fighter ? 0 : 1) + (s.claimed.background ? 0 : 1)
  const abilitiesValid = s.method !== "pointbuy" || method.remaining >= 0

  const status: Record<StageId, StageStatus> = {
    concept: s.name.trim()
      ? { state: "complete", detail: s.name.trim() }
      : { state: "incomplete", detail: "Needs a name" },
    origin:
      originMissing.length || originIssues.length
        ? {
            state: originIssues.length ? "blocked" : "incomplete",
            detail: originIssues.length
              ? `${originIssues.length} conflict${originIssues.length > 1 ? "s" : ""} to resolve`
              : `Needs ${originMissing.join(", ")}`,
          }
        : { state: "complete", detail: `${species.name} \u00b7 ${bg.name}` },
    class:
      classMissing.length || classIssues.length
        ? {
            state: classIssues.length ? "blocked" : "incomplete",
            detail: classIssues.length
              ? "1 conflict to resolve"
              : `Needs ${classMissing.join(", ")}`,
          }
        : { state: "complete", detail: "Fighter \u00b7 level 1" },
    abilities: abilitiesValid
      ? {
          state: "complete",
          detail: {
            standard: "Standard array",
            roll: "Recorded rolls",
            pointbuy: `Point buy \u00b7 ${method.remaining} left`,
          }[s.method],
        }
      : { state: "incomplete", detail: "Over the point budget" },
    equipment: unclaimed
      ? {
          state: "optional",
          detail: `${unclaimed} package${unclaimed > 1 ? "s" : ""} unclaimed`,
        }
      : {
          state: "complete",
          detail: `${gp} GP \u00b7 ${inventory.length} items`,
        },
    review: { state: "ready", detail: "" },
  }
  const blockers = (
    ["concept", "origin", "class", "abilities"] as StageId[]
  ).filter((id) => status[id].state !== "complete")
  status.review =
    blockers.length === 0
      ? { state: "ready", detail: "Ready to finish" }
      : {
          state: "incomplete",
          detail: `${blockers.length} stage${blockers.length > 1 ? "s" : ""} to finish`,
        }
  const completeCount =
    (["concept", "origin", "class", "abilities"] as StageId[]).length -
    blockers.length

  return {
    species,
    bg,
    prof,
    abilities,
    activePicks,
    dormant,
    skillSources,
    featSources,
    issues,
    inventory,
    gp,
    hasChain,
    hasSword,
    wearing,
    wielding,
    ac: { value: ac, parts: acParts },
    hp: { value: hp, parts: hpParts },
    initiative: { value: initiative, parts: initParts },
    speed,
    speedNotes,
    saves,
    skills,
    passive: 10 + perception,
    attack,
    method,
    status,
    blockers,
    completeCount,
  }
}

export type Derived = ReturnType<typeof derive>
