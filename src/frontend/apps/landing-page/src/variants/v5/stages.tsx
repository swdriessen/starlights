import { Lock, Minus, Plus } from "lucide-react"
import { useState } from "react"
import { cn } from "ui-framework"

import { fmt, pointBuySpent, type Method, type Scores } from "./compute"
import {
  abilityList,
  backgroundList,
  classRoster,
  fighter,
  languageOptions,
  pointCost,
  portraits,
  rolledPool,
  speciesList,
  standardArray,
  stages,
  type Ab,
  type StageId,
} from "./data"
import { field, type StageProps } from "./shared"
import { Chip, Notice, OptionCard, SectionTitle, StageIcon, Tag } from "./ui"

function toggle(list: string[], value: string, max: number) {
  if (list.includes(value)) return list.filter((v) => v !== value)
  return list.length >= max ? list : [...list, value]
}

/** Every conflict, wherever it was caused, with a jump to the stage that can resolve it. */
function Conflicts({
  d,
  here,
  go,
}: {
  d: StageProps["d"]
  here: StageId
  go: StageProps["go"]
}) {
  return (
    <Notice
      tone="warn"
      title={`${d.issues.length} conflict${d.issues.length > 1 ? "s" : ""}`}
    >
      <ul className="space-y-2">
        {d.issues.map((i) => (
          <li key={i.text} className="flex flex-wrap items-baseline gap-x-3">
            <span>{i.text}</span>
            {i.stage !== here && (
              <button
                type="button"
                onClick={() => go(i.stage)}
                className="text-ink underline decoration-gold underline-offset-4"
              >
                Fix in {stages.find((x) => x.id === i.stage)!.label}
              </button>
            )}
          </li>
        ))}
      </ul>
    </Notice>
  )
}

/* ------------------------------------------------------------------ Concept */

export function ConceptStage({ s, update }: StageProps) {
  return (
    <div className="space-y-10">
      <section className="space-y-5">
        <SectionTitle title="Name" hint="Autosaves as you type." />
        <label className="block">
          <span className="eyebrow text-mute">Character name</span>
          <input
            className={`${field} mt-2.5 text-base`}
            value={s.name}
            onChange={(e) => update((p) => ({ ...p, name: e.target.value }))}
            placeholder="Name your hero"
            aria-invalid={!s.name.trim()}
          />
          {!s.name.trim() && (
            <span className="mt-2 block text-sm text-gold">
              A name is required before you can finish.
            </span>
          )}
        </label>
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Portrait"
          hint="From your private library. Uploads live in Private files."
        />
        <div className="grid grid-cols-4 gap-3 sm:grid-cols-8">
          {portraits.map((src, i) => (
            <button
              key={src}
              type="button"
              aria-pressed={s.portrait === i}
              aria-label={`Portrait ${i + 1}`}
              onClick={() => update((p) => ({ ...p, portrait: i }))}
              className={cn(
                "aspect-square overflow-hidden rounded-sm border-2 transition",
                s.portrait === i
                  ? "border-gold"
                  : "border-transparent opacity-70 hover:opacity-100"
              )}
            >
              <img
                src={src}
                alt=""
                className="size-full object-cover"
                loading="lazy"
              />
            </button>
          ))}
        </div>
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Concept notes"
          hint="Optional. Never blocks the build."
        />
        <textarea
          className={`${field} min-h-28 leading-relaxed`}
          value={s.notes}
          onChange={(e) => update((p) => ({ ...p, notes: e.target.value }))}
        />
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Ruleset"
          hint="Pinned when the character was created."
        />
        <dl className="grid gap-px overflow-hidden rounded-sm border border-rule bg-rule text-sm sm:grid-cols-3">
          {[
            ["Game", "D&D 5e"],
            ["Content", "SRD 5.2.1 (pinned)"],
            ["Campaign", "None yet"],
          ].map(([k, v]) => (
            <div key={k} className="bg-sky px-4 py-3">
              <dt className="eyebrow text-mute">{k}</dt>
              <dd className="mt-2 font-medium">{v}</dd>
            </div>
          ))}
        </dl>
      </section>
    </div>
  )
}

/* ------------------------------------------------------------------- Origin */

export function OriginStage({ s, d, update, preview, pin, go }: StageProps) {
  const [q, setQ] = useState("")
  const [size, setSize] = useState<"All" | "Medium" | "Small">("All")
  const needle = q.trim().toLowerCase()
  const match = (text: string) => !needle || text.toLowerCase().includes(needle)

  const speciesShown = speciesList.filter(
    (x) =>
      (size === "All" || x.size === size) &&
      match(`${x.name} ${x.blurb} ${x.traits.map((t) => t.name).join(" ")}`)
  )
  const bgShown = backgroundList.filter((x) =>
    match(`${x.name} ${x.blurb} ${x.skills.join(" ")} ${x.feat}`)
  )
  const bonus = s.bonus[d.bg.id] ?? {}
  const setBonus = (patch: { two?: Ab; one?: Ab }) =>
    update((p) => ({
      ...p,
      bonus: { ...p.bonus, [d.bg.id]: { ...p.bonus[d.bg.id], ...patch } },
    }))

  return (
    <div className="space-y-10">
      <div className="flex flex-wrap items-center gap-3">
        <input
          type="search"
          aria-label="Search species and backgrounds"
          className={`${field} max-w-xs`}
          placeholder="Search species, backgrounds, traits"
          value={q}
          onChange={(e) => setQ(e.target.value)}
        />
        <div
          className="flex items-center gap-2"
          role="group"
          aria-label="Species size"
        >
          {(["All", "Medium", "Small"] as const).map((v) => (
            <Chip key={v} on={size === v} onClick={() => setSize(v)}>
              {v === "All" ? "Any size" : v}
            </Chip>
          ))}
        </div>
      </div>

      {d.issues.length > 0 && <Conflicts d={d} here="origin" go={go} />}

      <section className="space-y-5">
        <SectionTitle
          title="Species"
          hint={`${speciesShown.length} of ${speciesList.length} shown · SRD 5.2.1`}
        />
        <div className="grid gap-3 sm:grid-cols-2">
          {speciesShown.map((x) => (
            <OptionCard
              key={x.id}
              on={s.speciesId === x.id}
              onClick={() => {
                update((p) => ({ ...p, speciesId: x.id }))
                pin({ kind: "species", id: x.id })
              }}
              onHover={(over) =>
                preview(over ? { kind: "species", id: x.id } : null)
              }
            >
              <p className="display text-lg tracking-wide">{x.name}</p>
              <p className="mt-2 pr-5 text-sm leading-relaxed text-mute">
                {x.blurb}
              </p>
              <div className="mt-3 flex flex-wrap gap-1.5">
                <Tag>{x.size}</Tag>
                <Tag>{x.speed} ft</Tag>
                {x.hpPerLevel && <Tag>+{x.hpPerLevel} HP / level</Tag>}
              </div>
            </OptionCard>
          ))}
          {speciesShown.length === 0 && (
            <p className="text-sm text-mute">
              No species match. Clear the filters.
            </p>
          )}
        </div>

        {d.activePicks.length > 0 && (
          <div className="grid gap-4 rounded-sm border border-rule bg-ink/[0.03] p-4 sm:grid-cols-2">
            {d.activePicks.map((c) => (
              <label key={c.id} className="block">
                <span className="eyebrow text-mute">{c.label}</span>
                <select
                  className={`${field} mt-2.5`}
                  value={c.value ?? ""}
                  onChange={(e) =>
                    update((p) => ({
                      ...p,
                      picks: {
                        ...p.picks,
                        [`${s.speciesId}:${c.id}`]: e.target.value,
                      },
                    }))
                  }
                >
                  <option value="" disabled>
                    Choose…
                  </option>
                  {c.options.map((o) => (
                    <option key={o}>{o}</option>
                  ))}
                </select>
                {!c.value && (
                  <span className="mt-2 block text-sm text-gold">Required</span>
                )}
              </label>
            ))}
          </div>
        )}
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Background"
          hint={`${bgShown.length} of ${backgroundList.length} shown · SRD 5.2.1`}
        />
        <div className="grid gap-3 sm:grid-cols-2">
          {bgShown.map((x) => (
            <OptionCard
              key={x.id}
              on={s.backgroundId === x.id}
              onClick={() => {
                update((p) => ({ ...p, backgroundId: x.id }))
                pin({ kind: "background", id: x.id })
              }}
              onHover={(over) =>
                preview(over ? { kind: "background", id: x.id } : null)
              }
            >
              <p className="display text-lg tracking-wide">{x.name}</p>
              <p className="mt-2 pr-5 text-sm leading-relaxed text-mute">
                {x.blurb}
              </p>
              <div className="mt-3 flex flex-wrap gap-1.5">
                {x.skills.map((k) => (
                  <Tag key={k}>{k}</Tag>
                ))}
                <Tag>{x.feat}</Tag>
              </div>
            </OptionCard>
          ))}
        </div>

        <div className="space-y-5 rounded-sm border border-rule bg-ink/[0.03] p-4">
          <p className="text-sm leading-relaxed text-mute">
            {d.bg.name} lets you raise {d.bg.allowed.join(", ")}: one by 2 and a
            different one by 1.
          </p>
          {(
            [
              ["two", "+2 to"],
              ["one", "+1 to"],
            ] as const
          ).map(([slot, label]) => (
            <div key={slot} className="flex flex-wrap items-center gap-3">
              <span className="eyebrow w-14 text-mute">{label}</span>
              {d.bg.allowed.map((a) => (
                <Chip
                  key={a}
                  on={bonus[slot] === a}
                  disabled={slot === "two" ? bonus.one === a : bonus.two === a}
                  onClick={() =>
                    setBonus({ [slot]: bonus[slot] === a ? undefined : a })
                  }
                  onHover={(over) =>
                    preview(over ? { kind: "ability", id: a } : null)
                  }
                >
                  {a}
                </Chip>
              ))}
              {!bonus[slot] && (
                <span className="text-sm text-gold">Required</span>
              )}
            </div>
          ))}
        </div>
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Languages"
          hint="Common plus two more."
          aside={
            <span className="eyebrow text-mute">{s.languages.length} of 2</span>
          }
        />
        <div className="flex flex-wrap gap-2.5">
          <Chip on disabled>
            Common
          </Chip>
          {languageOptions.map((l) => (
            <Chip
              key={l}
              on={s.languages.includes(l)}
              disabled={!s.languages.includes(l) && s.languages.length >= 2}
              onClick={() =>
                update((p) => ({ ...p, languages: toggle(p.languages, l, 2) }))
              }
            >
              {l}
            </Chip>
          ))}
        </div>
      </section>

      {d.dormant.length > 0 && (
        <Notice title="Kept for later">
          These choices no longer apply but are retained. They return if you
          switch back.
          <ul className="mt-2.5 space-y-1">
            {d.dormant.map((x) => (
              <li key={x.label} className="flex justify-between gap-4">
                <span>{x.label}</span>
                <span className="text-ink">{x.value}</span>
              </li>
            ))}
          </ul>
        </Notice>
      )}
    </div>
  )
}

/* -------------------------------------------------------------------- Class */

export function ClassStage({ s, d, update, preview, pin, go }: StageProps) {
  return (
    <div className="space-y-10">
      <section className="space-y-5">
        <SectionTitle
          title="Class"
          hint="One class at level 1. The rest of the roster arrives slice by slice."
          aside={
            <span className="inline-flex items-center gap-2 text-sm text-mute">
              <Lock className="size-3.5" aria-hidden="true" /> Level 1 · levels
              2 to 20 arrive with F05
            </span>
          }
        />
        <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 lg:grid-cols-4">
          {classRoster.map((c) => (
            <button
              key={c.name}
              type="button"
              aria-pressed={c.ready}
              disabled={!c.ready}
              title={
                c.ready ? undefined : "Not available in this release slice yet"
              }
              onMouseEnter={() =>
                c.ready && preview({ kind: "class", id: c.name })
              }
              onMouseLeave={() => preview(null)}
              onClick={() => pin({ kind: "class", id: c.name })}
              className={cn(
                "rounded-sm border px-3 py-3 text-left transition-colors",
                c.ready
                  ? "border-gold bg-gold/[0.07]"
                  : "cursor-not-allowed border-rule opacity-45"
              )}
            >
              <p className="display text-base tracking-wide">{c.name}</p>
              <p className="mt-1.5 text-xs text-mute">
                {c.ready ? c.role : "Later"}
              </p>
            </button>
          ))}
        </div>
      </section>

      {d.issues.length > 0 && <Conflicts d={d} here="class" go={go} />}

      <section className="space-y-5">
        <SectionTitle
          title="Skill proficiencies"
          hint="Choose two."
          aside={
            <span className="eyebrow text-mute">
              {s.fighterSkills.length} of 2
            </span>
          }
        />
        <div className="flex flex-wrap gap-2.5">
          {fighter.skillOptions.map((k) => (
            <Chip
              key={k}
              on={s.fighterSkills.includes(k)}
              warn={
                s.fighterSkills.includes(k) &&
                (d.skillSources[k]?.length ?? 0) > 1
              }
              disabled={
                !s.fighterSkills.includes(k) && s.fighterSkills.length >= 2
              }
              title={
                (d.skillSources[k]?.length ?? 0) > 1
                  ? `Also granted by ${d.skillSources[k].filter((x) => x !== "Fighter").join(", ")}`
                  : undefined
              }
              onClick={() =>
                update((p) => ({
                  ...p,
                  fighterSkills: toggle(p.fighterSkills, k, 2),
                }))
              }
            >
              {k}
            </Chip>
          ))}
        </div>
      </section>

      <section className="space-y-5">
        <SectionTitle title="Fighting Style" hint="One style feat." />
        <div className="grid gap-3 sm:grid-cols-2">
          {fighter.styles.map((st) => (
            <OptionCard
              key={st.name}
              on={s.style === st.name}
              onClick={() => {
                update((p) => ({ ...p, style: st.name }))
                pin({ kind: "style", id: st.name })
              }}
              onHover={(over) =>
                preview(over ? { kind: "style", id: st.name } : null)
              }
            >
              <p className="display text-base tracking-wide">{st.name}</p>
              <p className="mt-2 pr-5 text-sm leading-relaxed text-mute">
                {st.text}
              </p>
            </OptionCard>
          ))}
        </div>
      </section>

      <section className="space-y-5">
        <SectionTitle
          title="Weapon Mastery"
          hint="Choose three weapon types whose mastery properties you can use."
          aside={
            <span className="eyebrow text-mute">{s.masteries.length} of 3</span>
          }
        />
        <div className="flex flex-wrap gap-2.5">
          {fighter.masteries.map((m) => (
            <Chip
              key={m.name}
              on={s.masteries.includes(m.name)}
              disabled={
                !s.masteries.includes(m.name) && s.masteries.length >= 3
              }
              onClick={() =>
                update((p) => ({
                  ...p,
                  masteries: toggle(p.masteries, m.name, 3),
                }))
              }
              onHover={(over) =>
                preview(over ? { kind: "mastery", id: m.name } : null)
              }
            >
              {m.name} <span className="ml-1 text-mute">· {m.mastery}</span>
            </Chip>
          ))}
        </div>
      </section>
    </div>
  )
}

/* ---------------------------------------------------------------- Abilities */

const methods: { id: Method; label: string; hint: string }[] = [
  {
    id: "standard",
    label: "Standard array",
    hint: "Assign 15, 14, 13, 12, 10 and 8 once each.",
  },
  {
    id: "pointbuy",
    label: "Point buy",
    hint: "27 points; scores from 8 to 15.",
  },
  {
    id: "roll",
    label: "Rolled",
    hint: "Six 4d6-drop-lowest results, saved so they never reroll.",
  },
]

export function AbilitiesStage({ s, d, update, preview }: StageProps) {
  const pool = s.method === "roll" ? rolledPool : standardArray
  const key: "standard" | "roll" | "pointbuy" = s.method
  const scores: Scores = s[key]

  const assign = (ab: Ab, value: number) =>
    update((p) => {
      const cur = { ...p[key] }
      const other = (Object.keys(cur) as Ab[]).find(
        (k) => k !== ab && cur[k] === value
      )
      if (other) cur[other] = cur[ab]
      cur[ab] = value
      return { ...p, [key]: cur }
    })
  const step = (ab: Ab, delta: number) =>
    update((p) => {
      const next = p.pointbuy[ab] + delta
      if (next < 8 || next > 15) return p
      const trial = { ...p.pointbuy, [ab]: next }
      if (pointBuySpent(trial) > 27) return p
      return { ...p, pointbuy: trial }
    })

  return (
    <div className="space-y-8">
      <div
        role="tablist"
        aria-label="Score generation method"
        className="flex flex-wrap gap-2.5"
      >
        {methods.map((m) => (
          <Chip
            key={m.id}
            on={s.method === m.id}
            onClick={() => update((p) => ({ ...p, method: m.id }))}
          >
            {m.label}
          </Chip>
        ))}
      </div>
      <p className="text-sm text-mute">
        {methods.find((m) => m.id === s.method)!.hint}
      </p>

      {s.method === "pointbuy" && (
        <div>
          <div className="flex items-baseline justify-between text-sm">
            <span className="eyebrow text-mute">Budget</span>
            <span>
              <span className="display text-xl text-gold">
                {d.method.remaining}
              </span>{" "}
              of 27 points left
            </span>
          </div>
          <div className="mt-3 h-1.5 overflow-hidden rounded-full bg-ink/10">
            <div
              className="h-full bg-gold transition-all"
              style={{ width: `${(d.method.spent / 27) * 100}%` }}
            />
          </div>
        </div>
      )}

      {s.method !== "pointbuy" && (
        <div className="flex flex-wrap items-center gap-2">
          <span className="eyebrow mr-2 text-mute">
            {s.method === "roll" ? "Rolled" : "Values"}
          </span>
          {pool.map((v) => (
            <span
              key={v}
              className={cn(
                "display rounded-sm border px-3 py-1 text-lg",
                Object.values(scores).includes(v)
                  ? "border-rule text-mute"
                  : "border-gold text-gold"
              )}
            >
              {v}
            </span>
          ))}
          {s.method === "roll" && (
            <button
              type="button"
              disabled
              className="ml-auto text-sm text-mute underline decoration-dotted underline-offset-4"
              title="Rerolling asks for confirmation before replacing saved results"
            >
              Reroll…
            </button>
          )}
        </div>
      )}

      <div className="overflow-hidden rounded-sm border border-rule">
        <table className="w-full text-left text-sm">
          <caption className="sr-only">Ability scores</caption>
          <thead>
            <tr className="border-b border-rule text-mute">
              <th className="eyebrow px-4 py-3 font-semibold">Ability</th>
              <th className="eyebrow px-4 py-3 font-semibold">Base</th>
              <th className="eyebrow px-4 py-3 font-semibold">Origin</th>
              <th className="eyebrow px-4 py-3 text-right font-semibold">
                Score
              </th>
              <th className="eyebrow px-4 py-3 text-right font-semibold">
                Mod
              </th>
            </tr>
          </thead>
          <tbody>
            {abilityList.map((a) => {
              const r = d.abilities[a.key]
              return (
                <tr
                  key={a.key}
                  className="border-b border-rule last:border-0"
                  onMouseEnter={() => preview({ kind: "ability", id: a.key })}
                  onMouseLeave={() => preview(null)}
                >
                  <th scope="row" className="px-4 py-3 font-medium">
                    <span className="display mr-2 text-base text-gold">
                      {a.key}
                    </span>
                    <span className="text-mute">{a.name}</span>
                  </th>
                  <td className="px-4 py-2">
                    {s.method === "pointbuy" ? (
                      <div className="inline-flex items-center gap-1">
                        <button
                          type="button"
                          aria-label={`Lower ${a.name}`}
                          className="grid size-8 place-items-center rounded-sm border border-rule hover:border-ink/40 disabled:opacity-40"
                          disabled={scores[a.key] <= 8}
                          onClick={() => step(a.key, -1)}
                        >
                          <Minus className="size-3.5" aria-hidden="true" />
                        </button>
                        <span className="display w-8 text-center text-lg">
                          {scores[a.key]}
                        </span>
                        <button
                          type="button"
                          aria-label={`Raise ${a.name}`}
                          className="grid size-8 place-items-center rounded-sm border border-rule hover:border-ink/40 disabled:opacity-40"
                          disabled={
                            scores[a.key] >= 15 ||
                            d.method.remaining <
                              (pointCost[scores[a.key] + 1] ?? 99) -
                                pointCost[scores[a.key]]
                          }
                          onClick={() => step(a.key, 1)}
                        >
                          <Plus className="size-3.5" aria-hidden="true" />
                        </button>
                        <span className="ml-2 text-xs text-mute">
                          {pointCost[scores[a.key]]} pts
                        </span>
                      </div>
                    ) : (
                      <select
                        aria-label={`${a.name} base score`}
                        className={`${field} w-20 py-1.5`}
                        value={scores[a.key]}
                        onChange={(e) => assign(a.key, Number(e.target.value))}
                      >
                        {pool.map((v) => (
                          <option key={v}>{v}</option>
                        ))}
                      </select>
                    )}
                  </td>
                  <td className="px-4 py-3 text-mute">
                    {r.inc ? `+${r.inc} ${d.bg.name}` : "\u2014"}
                  </td>
                  <td className="display px-4 py-3 text-right text-xl">
                    {r.final}
                  </td>
                  <td className="display px-4 py-3 text-right text-xl text-gold">
                    {fmt(r.mod)}
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
      <p className="text-sm text-mute">
        Changing a value that is already in use swaps the two abilities. Origin
        increases come from your background and are never rolled into the base.
      </p>
    </div>
  )
}

/* ---------------------------------------------------------------- Equipment */

export function EquipmentStage({ s, d, update, preview }: StageProps) {
  const packages = [
    {
      id: "fighter" as const,
      title: "Fighter starting equipment",
      choice: s.fighterPackage,
      key: "fighterPackage" as const,
      a: [...fighter.fighterEquipA, `${fighter.fighterEquipAGp} GP`],
      b: `${fighter.fighterEquipBGp} GP`,
    },
    {
      id: "background" as const,
      title: `${d.bg.name} starting equipment`,
      choice: s.backgroundPackage,
      key: "backgroundPackage" as const,
      a: [...d.bg.equipA.items, `${d.bg.equipA.gp} GP`],
      b: `${d.bg.equipBGp} GP`,
    },
  ]
  return (
    <div className="space-y-10">
      <Notice title="Choosing is not owning">
        Picking a package records your intent only. Nothing is added to your
        character until you claim it.
      </Notice>

      <div className="grid gap-4 lg:grid-cols-2">
        {packages.map((p) => {
          const claimed = s.claimed[p.id]
          return (
            <section
              key={p.id}
              className="flex flex-col rounded-sm border border-rule p-4"
            >
              <h3 className="display text-lg tracking-wide">{p.title}</h3>
              <div className="mt-4 flex gap-2">
                {(["A", "B"] as const).map((v) => (
                  <Chip
                    key={v}
                    on={p.choice === v}
                    disabled={claimed}
                    onClick={() => update((pr) => ({ ...pr, [p.key]: v }))}
                  >
                    Package {v}
                  </Chip>
                ))}
              </div>
              <ul className="mt-4 flex-1 space-y-1.5 text-sm text-mute">
                {p.choice === "A" ? (
                  p.a.map((i) => <li key={i}>{i}</li>)
                ) : (
                  <li>{p.b} instead of items</li>
                )}
              </ul>
              <button
                type="button"
                onClick={() =>
                  update((pr) => ({
                    ...pr,
                    claimed: { ...pr.claimed, [p.id]: !pr.claimed[p.id] },
                  }))
                }
                className={cn(
                  "mt-5 w-full rounded-sm px-4 py-2.5 text-[0.8125rem] font-semibold tracking-[0.12em] uppercase transition",
                  claimed ? "border border-gold text-gold" : "btn-solid"
                )}
              >
                {claimed ? "Claimed · tap to remove" : "Claim package"}
              </button>
              {claimed && (
                <p className="mt-2 text-xs text-mute">
                  A claimed package can only be changed through a correction
                  preview (planned).
                </p>
              )}
            </section>
          )
        })}
      </div>

      <section className="space-y-5">
        <SectionTitle
          title="Possessions"
          hint="Owning and wearing are separate."
          aside={<span className="display text-xl text-gold">{d.gp} GP</span>}
        />
        {d.inventory.length === 0 ? (
          <p className="text-sm text-mute">Nothing claimed yet.</p>
        ) : (
          <ul className="divide-y divide-rule rounded-sm border border-rule text-sm">
            {d.inventory.map((i) => (
              <li key={i.name} className="flex items-center gap-4 px-4 py-3">
                <span className="w-8 text-mute tabular-nums">{i.qty}×</span>
                <span className="flex-1">{i.name}</span>
                {i.kind === "armor" && (
                  <Chip
                    on={s.worn}
                    onClick={() => update((p) => ({ ...p, worn: !p.worn }))}
                  >
                    {s.worn ? "Worn" : "Wear"}
                  </Chip>
                )}
                {i.kind === "weapon" && (
                  <Chip
                    on={s.wielded}
                    onClick={() =>
                      update((p) => ({ ...p, wielded: !p.wielded }))
                    }
                  >
                    {s.wielded ? "Wielded" : "Wield"}
                  </Chip>
                )}
              </li>
            ))}
          </ul>
        )}
      </section>

      <section
        className="rounded-sm border border-rule bg-ink/[0.03] p-4"
        onMouseEnter={() => preview(null)}
      >
        <p className="eyebrow text-gold">Why armor class {d.ac.value}?</p>
        <p className="mt-3 text-sm leading-relaxed text-mute">
          {d.ac.parts.join(" + ")} ={" "}
          <span className="display text-lg text-ink">{d.ac.value}</span>
          {!d.wearing &&
            d.hasChain &&
            " (your chain mail is owned but not worn)"}
        </p>
      </section>
    </div>
  )
}

/* ------------------------------------------------------------------- Review */

export function ReviewStage({ d, go, s }: StageProps) {
  const rows = stages.filter((x) => x.id !== "review")
  const ready = d.blockers.length === 0
  return (
    <div className="space-y-10">
      <section className="space-y-5">
        <SectionTitle
          title="Checklist"
          hint="Optional details and unclaimed equipment never block a build."
          aside={
            <span className="eyebrow text-mute">
              {d.completeCount} of 4 required
            </span>
          }
        />
        <ul className="divide-y divide-rule rounded-sm border border-rule">
          {rows.map((r) => {
            const st = d.status[r.id]
            return (
              <li key={r.id}>
                <button
                  type="button"
                  onClick={() => go(r.id)}
                  className="flex w-full items-center gap-4 px-4 py-3.5 text-left hover:bg-ink/[0.03]"
                >
                  <StageIcon state={st.state} />
                  <span className="display w-28 text-base tracking-wide">
                    {r.label}
                  </span>
                  <span className="flex-1 text-sm text-mute">{st.detail}</span>
                  <span className="eyebrow text-mute">
                    {st.state === "complete"
                      ? "Done"
                      : st.state === "optional"
                        ? "Optional"
                        : "Fix"}
                  </span>
                </button>
              </li>
            )
          })}
        </ul>
      </section>

      {d.issues.length > 0 && (
        <Notice
          tone="warn"
          title={`${d.issues.length} conflict${d.issues.length > 1 ? "s" : ""}`}
        >
          <ul className="space-y-2">
            {d.issues.map((i) => (
              <li key={i.text} className="flex gap-3">
                <button
                  type="button"
                  className="underline decoration-gold underline-offset-4"
                  onClick={() => go(i.stage)}
                >
                  {stages.find((x) => x.id === i.stage)!.label}
                </button>
                <span>{i.text}</span>
              </li>
            ))}
          </ul>
        </Notice>
      )}

      <div className="flex flex-wrap items-center gap-4">
        <button
          type="button"
          disabled={!ready}
          className={cn(
            "btn-solid rounded-sm px-6 py-3.5",
            !ready && "cursor-not-allowed opacity-45"
          )}
        >
          Finish build
        </button>
        <button
          type="button"
          disabled
          className="btn-line cursor-not-allowed rounded-sm px-6 py-3.5 opacity-45"
          title="PDF export is planned (F18)"
        >
          Export PDF · planned
        </button>
        <span className="text-sm text-mute">
          {ready
            ? `${s.name} is ready for the table.`
            : "Resolve the items above to finish."}
        </span>
      </div>
    </div>
  )
}
