import { StarMark } from "../../components/glyphs"
import { cn } from "ui-framework"

import { fmt, type BuildState, type Derived } from "./compute"
import {
  abilityList,
  backgroundList,
  feats,
  fighter,
  speciesList,
  stages,
  type StageId,
} from "./data"
import type { Focus } from "./shared"
import { Tag } from "./ui"

type Doc = {
  eyebrow: string
  title: string
  facts?: [string, string][]
  paragraphs: string[]
  grants?: { name: string; text: string }[]
  note?: string
}

const stageHelp: Record<StageId, { paragraphs: string[]; tips: string[] }> = {
  concept: {
    paragraphs: [
      "Start with who this character is. A name and a portrait are all that is required; everything else on this page is optional and saved as you go.",
    ],
    tips: [
      "Portraits come from your private library.",
      "Notes are for you and never affect the sheet.",
    ],
  },
  origin: {
    paragraphs: [
      "Origin is where your character comes from. Pick a species and a background, then make the small choices they ask for.",
      "Hover any option to read it here without choosing it. Switching later never deletes your earlier choices; they are kept and come back if you return.",
    ],
    tips: [
      "Search covers names, traits, skills and feats.",
      "Conflicts, like the same skill granted twice, are explained before they cost you.",
    ],
  },
  class: {
    paragraphs: [
      "Your class decides what you are trained to do. At level 1 a Fighter picks two skills, a Fighting Style and three weapon masteries.",
    ],
    tips: [
      "Other classes arrive in later slices.",
      "Skill overlaps with your origin are flagged for you to fix.",
    ],
  },
  abilities: {
    paragraphs: [
      "Choose how to generate your six ability scores. Standard array, point buy and saved rolls all feed the same sheet.",
      "Your background's increases are shown separately so you can always see what came from where.",
    ],
    tips: [
      "Rolled results are saved and never silently rerolled.",
      "Hover an ability to read what it governs.",
    ],
  },
  equipment: {
    paragraphs: [
      "Starting equipment is a choice you make and then claim. Owning an item and wearing it are separate, and the sheet only counts what you actually wear or wield.",
    ],
    tips: [
      "Try taking the chain mail off and watch armor class change.",
      "Claimed packages are never duplicated when you reopen.",
    ],
  },
  review: {
    paragraphs: [
      "Review checks every stage. You can finish once the four required stages are complete.",
    ],
    tips: ["Equipment is optional and never blocks a build."],
  },
}

function describe(focus: NonNullable<Focus>): Doc {
  switch (focus.kind) {
    case "species": {
      const x = speciesList.find((s) => s.id === focus.id)!
      return {
        eyebrow: "Species · SRD 5.2.1",
        title: x.name,
        facts: [
          ["Size", x.size],
          ["Speed", `${x.speed} ft`],
          ["Traits", String(x.traits.length)],
        ],
        paragraphs: x.about,
        grants: x.traits,
        note: x.choices.length
          ? `You will choose: ${x.choices.map((c) => c.label.split(":")[0]).join(", ")}.`
          : undefined,
      }
    }
    case "background": {
      const x = backgroundList.find((b) => b.id === focus.id)!
      return {
        eyebrow: "Background · SRD 5.2.1",
        title: x.name,
        facts: [
          ["Ability increases", x.allowed.join(" · ")],
          ["Skills", x.skills.join(", ")],
          ["Tool", x.tool],
          ["Origin feat", x.feat],
        ],
        paragraphs: x.about,
        grants: [{ name: x.feat, text: feats[x.feat].text }],
        note: "Starting equipment: package A or 50 GP.",
      }
    }
    case "feat": {
      const f = feats[focus.id]
      return { eyebrow: "Feat · Origin", title: f.name, paragraphs: [f.text] }
    }
    case "style": {
      const st = fighter.styles.find((x) => x.name === focus.id)!
      return {
        eyebrow: "Fighting Style · Fighter 1",
        title: st.name,
        paragraphs: [st.text],
      }
    }
    case "mastery": {
      const m = fighter.masteries.find((x) => x.name === focus.id)!
      return {
        eyebrow: "Weapon Mastery",
        title: m.name,
        facts: [["Mastery", m.mastery]],
        paragraphs: [m.text],
      }
    }
    case "ability": {
      const a = abilityList.find((x) => x.key === focus.id)!
      return {
        eyebrow: "Ability",
        title: `${a.name} (${a.key})`,
        paragraphs: [a.blurb],
      }
    }
    case "class":
      return {
        eyebrow: "Class · SRD 5.2.1",
        title: fighter.name,
        facts: [
          ["Hit die", `d${fighter.hitDie}`],
          ["Saving throws", "STR, CON"],
          ["Level 1", "Style, Second Wind, Mastery"],
        ],
        paragraphs: fighter.about,
        grants: [
          {
            name: "Second Wind",
            text: "As a bonus action, regain d10 + your Fighter level hit points. You have two uses, refreshed by rests.",
          },
          {
            name: "Weapon Mastery",
            text: "Use the mastery properties of three weapon types you choose.",
          },
        ],
      }
  }
}

export function DescriptionPanel({
  focus,
  stage,
  d,
}: {
  focus: Focus
  stage: StageId
  d: Derived
}) {
  const st = stages.find((x) => x.id === stage)!
  if (!focus) {
    const help = stageHelp[stage]
    const issues = d.issues.filter((i) => i.stage === stage)
    return (
      <div className="space-y-6">
        <header>
          <p className="eyebrow text-gold">About this stage</p>
          <h2 className="display mt-4 text-3xl">{st.label}</h2>
        </header>
        {help.paragraphs.map((p) => (
          <p key={p} className="leading-relaxed text-mute">
            {p}
          </p>
        ))}
        <ul className="space-y-2.5 border-t border-rule pt-5 text-sm text-mute">
          {help.tips.map((t) => (
            <li key={t} className="flex gap-3">
              <StarMark className="mt-1 size-3 shrink-0" />
              {t}
            </li>
          ))}
        </ul>
        {issues.length > 0 && (
          <div className="rounded-sm border border-gold/60 bg-gold/[0.07] p-4 text-sm leading-relaxed text-mute">
            <p className="eyebrow mb-2 text-gold">Needs attention</p>
            {issues.map((i) => (
              <p key={i.text}>{i.text}</p>
            ))}
          </div>
        )}
        <p className="text-xs text-mute">Hover an option to read it here.</p>
      </div>
    )
  }
  const doc = describe(focus)
  return (
    <div className="space-y-6">
      <header>
        <p className="eyebrow text-gold">{doc.eyebrow}</p>
        <h2 className="display mt-4 text-3xl leading-[1.02]">{doc.title}</h2>
      </header>
      {doc.facts && (
        <dl className="grid grid-cols-2 gap-px overflow-hidden rounded-sm border border-rule bg-rule text-sm">
          {doc.facts.map(([k, v]) => (
            <div key={k} className="bg-deep px-3.5 py-3 odd:last:col-span-2">
              <dt className="eyebrow text-mute">{k}</dt>
              <dd className="mt-2 font-medium">{v}</dd>
            </div>
          ))}
        </dl>
      )}
      {doc.paragraphs.map((p) => (
        <p key={p} className="leading-relaxed text-mute">
          {p}
        </p>
      ))}
      {doc.grants && (
        <div>
          <p className="eyebrow border-b border-rule pb-3 text-mute">Grants</p>
          <ul className="divide-y divide-rule">
            {doc.grants.map((g) => (
              <li key={g.name} className="py-3.5">
                <p className="display text-base tracking-wide">{g.name}</p>
                <p className="mt-1.5 text-sm leading-relaxed text-mute">
                  {g.text}
                </p>
              </li>
            ))}
          </ul>
        </div>
      )}
      {doc.note && (
        <p className="border-l-2 border-gold pl-4 text-sm leading-relaxed text-mute">
          {doc.note}
        </p>
      )}
      <p className="text-xs text-mute">
        Mock text for the prototype, not shipped content.
      </p>
    </div>
  )
}

function Stat({
  label,
  value,
  sub,
}: {
  label: string
  value: string
  sub?: string
}) {
  return (
    <div className="rounded-sm border border-rule bg-sky px-3.5 py-3">
      <p className="eyebrow text-mute">{label}</p>
      <p className="display mt-2.5 text-3xl tabular-nums">{value}</p>
      {sub && <p className="mt-1.5 text-xs leading-snug text-mute">{sub}</p>}
    </div>
  )
}

export function SheetPanel({ s, d }: { s: BuildState; d: Derived }) {
  const proficient = d.skills.filter((x) => x.proficient)
  return (
    <div className="space-y-7">
      <header>
        <p className="eyebrow text-gold">Live sheet · recalculated</p>
        <h2 className="display mt-4 text-3xl leading-[1.02]">
          {s.name || "Unnamed"}
        </h2>
        <p className="mt-2 text-sm text-mute">
          {d.species.name} Fighter 1 · {d.bg.name}
        </p>
      </header>

      <div className="grid grid-cols-3 gap-2.5">
        {abilityList.map((a) => {
          const r = d.abilities[a.key]
          return (
            <div
              key={a.key}
              className="rounded-sm border border-rule px-2 py-3 text-center"
              title={`${r.base}${r.inc ? ` + ${r.inc} ${d.bg.name}` : ""}`}
            >
              <p className="eyebrow text-mute">{a.key}</p>
              <p className="display mt-2.5 text-3xl tabular-nums">
                {fmt(r.mod)}
              </p>
              <p className="mt-1.5 text-xs text-mute tabular-nums">
                {r.final}
                {r.inc ? (
                  <span className="text-gold">
                    {" "}
                    ({r.base}+{r.inc})
                  </span>
                ) : null}
              </p>
            </div>
          )
        })}
      </div>

      <div className="grid grid-cols-2 gap-2.5">
        <Stat
          label="Armor class"
          value={String(d.ac.value)}
          sub={d.ac.parts.join(" + ")}
        />
        <Stat
          label="Hit points"
          value={String(d.hp.value)}
          sub={d.hp.parts.join(" + ")}
        />
        <Stat
          label="Initiative"
          value={fmt(d.initiative.value)}
          sub={d.initiative.parts.join(" · ")}
        />
        <Stat
          label="Speed"
          value={`${d.speed} ft`}
          sub={d.speedNotes[0] ?? `${d.species.size}`}
        />
        <Stat label="Proficiency" value={fmt(d.prof)} sub="Level 1" />
        <Stat label="Passive Perception" value={String(d.passive)} />
      </div>

      <section>
        <p className="eyebrow border-b border-rule pb-3 text-mute">
          Saving throws
        </p>
        <ul className="grid grid-cols-2 gap-x-6 text-sm">
          {d.saves.map((x) => (
            <li
              key={x.key}
              className="flex items-center gap-2.5 border-b border-rule py-2"
            >
              <Dot on={x.prof} />
              <span className="flex-1">{x.key}</span>
              <span className="tabular-nums">{fmt(x.value)}</span>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <p className="eyebrow border-b border-rule pb-3 text-mute">
          Skills · {proficient.length} proficient
        </p>
        <ul className="grid grid-cols-2 gap-x-6 text-sm">
          {d.skills.map((x) => (
            <li
              key={x.name}
              className={cn(
                "flex items-center gap-2.5 border-b border-rule py-2",
                !x.proficient && "text-mute"
              )}
              title={
                x.proficient
                  ? `From ${d.skillSources[x.name].join(", ")}`
                  : undefined
              }
            >
              <Dot on={x.proficient} />
              <span className="flex-1 truncate">{x.name}</span>
              <span className="tabular-nums">{fmt(x.value)}</span>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <p className="eyebrow border-b border-rule pb-3 text-mute">Attacks</p>
        {d.attack ? (
          <table className="mt-2 w-full text-left text-sm">
            <tbody>
              <tr>
                <th scope="row" className="py-2 font-medium">
                  {d.attack.name}
                </th>
                <td className="py-2 tabular-nums">{fmt(d.attack.hit)}</td>
                <td className="py-2 text-mute">{d.attack.damage}</td>
                <td className="py-2 text-right text-mute">
                  {d.attack.mastery}
                </td>
              </tr>
            </tbody>
          </table>
        ) : (
          <p className="py-3 text-sm text-mute">No weapon wielded.</p>
        )}
      </section>

      <section>
        <p className="eyebrow border-b border-rule pb-3 text-mute">
          Features and feats
        </p>
        <div className="mt-3 flex flex-wrap gap-2">
          {d.featSources.map((f) => (
            <span
              key={f.name + f.source}
              title={f.source}
              className="rounded-sm border border-rule px-2.5 py-1 text-[0.8125rem]"
            >
              {f.name.replace("Fighting Style: ", "")}
              <span className="ml-1.5 text-mute">
                · {f.source.split(":")[0]}
              </span>
            </span>
          ))}
          <span className="rounded-sm border border-rule px-2.5 py-1 text-[0.8125rem]">
            Second Wind<span className="ml-1.5 text-mute">· 2 uses</span>
          </span>
          {d.species.traits.slice(0, 3).map((t) => (
            <Tag key={t.name}>{t.name}</Tag>
          ))}
        </div>
      </section>

      <section>
        <p className="eyebrow border-b border-rule pb-3 text-mute">
          Equipment · {d.gp} GP
        </p>
        {d.inventory.length === 0 ? (
          <p className="py-3 text-sm text-mute">Nothing claimed.</p>
        ) : (
          <ul className="mt-2 columns-2 gap-6 text-sm">
            {d.inventory.map((i) => (
              <li key={i.name} className="break-inside-avoid py-1 text-mute">
                {i.qty > 1 ? `${i.qty}× ` : ""}
                <span className="text-ink">{i.name}</span>
                {i.kind === "armor" && d.wearing && (
                  <span className="text-gold"> · worn</span>
                )}
                {i.kind === "weapon" && d.wielding && (
                  <span className="text-gold"> · wielded</span>
                )}
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  )
}

function Dot({ on }: { on: boolean }) {
  return (
    <span
      aria-label={on ? "Proficient" : "Not proficient"}
      className={cn(
        "size-2 shrink-0 rounded-full border",
        on ? "border-gold bg-gold" : "border-ink/30"
      )}
    />
  )
}
