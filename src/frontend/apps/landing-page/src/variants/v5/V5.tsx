import { Moon, Sun } from "lucide-react"
import { useEffect, useRef, useState } from "react"
import { Button, cn, useTheme } from "ui-framework"

import { StarMark } from "../../components/glyphs"
import { Starfield } from "../../components/starfield"
import { VariantLinks } from "../variant-links"
import {
  derive,
  fmt,
  initialState,
  type BuildState,
  type Derived,
} from "./compute"
import { portraits, stages, type StageId } from "./data"
import { DescriptionPanel, SheetPanel } from "./panel"
import type { Focus, StageProps } from "./shared"
import {
  AbilitiesStage,
  ClassStage,
  ConceptStage,
  EquipmentStage,
  OriginStage,
  ReviewStage,
} from "./stages"
import { StageIcon } from "./ui"
import "../v4/v4.css"
import "./v5.css"

const views: Record<StageId, (p: StageProps) => React.ReactNode> = {
  concept: ConceptStage,
  origin: OriginStage,
  class: ClassStage,
  abilities: AbilitiesStage,
  equipment: EquipmentStage,
  review: ReviewStage,
}

function TopBar({
  name,
  saving,
  dirty,
}: {
  name: string
  saving: boolean
  dirty: boolean
}) {
  const { setTheme } = useTheme()
  return (
    <header className="z-20 flex h-14 shrink-0 items-center gap-4 border-b border-rule bg-sky px-4 sm:px-6">
      <a
        href="#"
        className="display flex items-center gap-3 text-lg tracking-[0.28em]"
      >
        <StarMark className="size-4" />{" "}
        <span className="hidden sm:inline">Starlights</span>
      </a>
      <nav
        aria-label="Breadcrumb"
        className="flex min-w-0 items-center gap-2.5 text-sm text-mute"
      >
        <span className="hidden sm:inline">Characters</span>
        <span aria-hidden="true" className="hidden sm:inline">
          /
        </span>
        <span className="truncate font-medium text-ink">
          {name || "Unnamed"}
        </span>
        <span className="eyebrow hidden rounded-sm border border-gold/60 px-2 py-1 text-gold md:inline">
          Mockup
        </span>
      </nav>
      <div className="ml-auto flex items-center gap-4 text-sm text-mute">
        <span
          role="status"
          className="hidden items-center gap-2 sm:inline-flex"
        >
          <span
            className={cn(
              "size-1.5 rounded-full",
              saving ? "animate-pulse bg-gold" : "bg-gold"
            )}
          />
          {saving ? "Saving…" : dirty ? "All changes saved" : "Saved"}
        </span>
        <span className="eyebrow hidden rounded-sm border border-rule px-2.5 py-1.5 md:inline">
          Calculated
        </span>
        <Button
          variant="ghost"
          size="icon-lg"
          aria-label="Toggle light and dark theme"
          onClick={() =>
            setTheme(
              document.documentElement.classList.contains("dark")
                ? "light"
                : "dark"
            )
          }
        >
          <Sun className="hidden size-4 dark:block" aria-hidden="true" />
          <Moon className="size-4 dark:hidden" aria-hidden="true" />
        </Button>
        <span
          className="display grid size-8 place-items-center rounded-full border border-rule text-sm"
          title="Signed in as demo"
        >
          D
        </span>
      </div>
    </header>
  )
}

function StageNav({
  stage,
  setStage,
  d,
  s,
  compact,
}: {
  stage: StageId
  setStage: (id: StageId) => void
  d: Derived
  s: BuildState
  compact?: boolean
}) {
  if (compact) {
    return (
      <nav
        aria-label="Build stages"
        className="flex shrink-0 gap-1 overflow-x-auto border-b border-rule bg-deep px-3 xl:hidden"
      >
        {stages.map((x, i) => (
          <button
            key={x.id}
            type="button"
            aria-current={stage === x.id ? "step" : undefined}
            onClick={() => setStage(x.id)}
            className={cn(
              "flex shrink-0 items-center gap-2.5 border-b-2 px-3 py-3.5 text-sm transition-colors",
              stage === x.id
                ? "border-gold text-ink"
                : "border-transparent text-mute hover:text-ink"
            )}
          >
            <StageIcon state={d.status[x.id].state} />
            <span className="display tracking-wide">
              {i + 1}. {x.label}
            </span>
          </button>
        ))}
      </nav>
    )
  }
  return (
    <aside className="relative hidden w-[17.5rem] shrink-0 flex-col overflow-y-auto border-r border-rule bg-deep xl:flex">
      <Starfield
        count={40}
        seed={9}
        className="[mask-image:linear-gradient(to_bottom,transparent,black_60%)]"
      />
      <div className="border-b border-rule p-5">
        <div className="flex items-center gap-4">
          <img
            src={portraits[s.portrait]}
            alt=""
            className="size-16 rounded-sm border-2 border-rule object-cover"
          />
          <div className="min-w-0">
            <p className="display truncate text-xl leading-tight">
              {s.name || "Unnamed"}
            </p>
            <p className="mt-1.5 text-xs text-mute">
              {d.species.name} Fighter 1
            </p>
          </div>
        </div>
        <dl className="mt-4 grid grid-cols-3 gap-px overflow-hidden rounded-sm border border-rule bg-rule text-center">
          {[
            ["AC", String(d.ac.value)],
            ["HP", String(d.hp.value)],
            ["Init", fmt(d.initiative.value)],
          ].map(([k, v]) => (
            <div key={k} className="bg-deep py-2.5">
              <dt className="eyebrow text-mute">{k}</dt>
              <dd className="display mt-1.5 text-xl tabular-nums">{v}</dd>
            </div>
          ))}
        </dl>
      </div>

      <nav aria-label="Build stages" className="p-3">
        <p className="eyebrow px-2 pt-2 pb-3 text-mute">
          Build · {d.completeCount} of 4 required
        </p>
        <ol className="space-y-1">
          {stages.map((x, i) => {
            const st = d.status[x.id]
            return (
              <li key={x.id}>
                <button
                  type="button"
                  aria-current={stage === x.id ? "step" : undefined}
                  onClick={() => setStage(x.id)}
                  className={cn(
                    "flex w-full items-start gap-3 rounded-sm border px-3 py-3 text-left transition-colors",
                    stage === x.id
                      ? "border-gold/60 bg-gold/[0.07]"
                      : "border-transparent hover:bg-ink/[0.04]"
                  )}
                >
                  <span className="display mt-0.5 w-4 text-sm text-mute">
                    {i + 1}
                  </span>
                  <span className="min-w-0 flex-1">
                    <span className="display block text-base tracking-wide">
                      {x.label}
                    </span>
                    <span
                      className={cn(
                        "mt-1 block truncate text-xs",
                        st.state === "blocked" || st.state === "incomplete"
                          ? "text-gold"
                          : "text-mute"
                      )}
                    >
                      {st.detail || x.hint}
                    </span>
                  </span>
                  <span className="mt-1">
                    <StageIcon state={st.state} />
                  </span>
                </button>
              </li>
            )
          })}
        </ol>
      </nav>

      {d.dormant.length > 0 && (
        <div className="mx-3 mb-3 rounded-sm border border-rule p-3.5">
          <p className="eyebrow text-mute">
            Kept for later · {d.dormant.length}
          </p>
          <ul className="mt-3 space-y-2 text-xs text-mute">
            {d.dormant.map((x) => (
              <li key={x.label}>
                <span className="text-ink">{x.label}</span>
                <br />
                {x.value}
              </li>
            ))}
          </ul>
        </div>
      )}

      <div className="mt-auto border-t border-rule p-5 text-xs text-mute">
        <p className="leading-relaxed">
          Mock data. Nothing here is saved or sent to the API.
        </p>
        <VariantLinks
          current="v5"
          className="mt-3"
          linkClass="hover:text-ink"
          activeClass="text-gold underline underline-offset-4"
        />
      </div>
    </aside>
  )
}

export default function V5() {
  const [s, setS] = useState<BuildState>(initialState)
  const [stage, setStage] = useState<StageId>("origin")
  const [hovered, setHovered] = useState<Focus>(null)
  const [pinned, setPinned] = useState<Focus>({
    kind: "background",
    id: "soldier",
  })
  const [tab, setTab] = useState<"description" | "sheet">("description")
  const [saving, setSaving] = useState(false)
  const [dirty, setDirty] = useState(false)
  const first = useRef(true)
  const scroller = useRef<HTMLDivElement>(null)
  const d = derive(s)

  // Autosave indicator: pending while edits land, settled shortly after.
  useEffect(() => {
    if (first.current) {
      first.current = false
      return undefined
    }
    setSaving(true)
    setDirty(true)
    const t = setTimeout(() => setSaving(false), 700)
    return () => clearTimeout(t)
  }, [s])

  const go = (id: StageId) => {
    setStage(id)
    setPinned(null)
    setHovered(null)
    scroller.current?.scrollTo({ top: 0 })
  }
  const focus = hovered ?? pinned
  const props: StageProps = {
    s,
    d,
    update: setS,
    preview: setHovered,
    pin: (f) => {
      setPinned(f)
      setTab("description")
    },
    go,
  }
  const idx = stages.findIndex((x) => x.id === stage)
  const current = stages[idx]
  const View = views[stage]

  return (
    <div className="v5 flex min-h-svh flex-col lg:h-svh lg:overflow-hidden">
      <TopBar name={s.name} saving={saving} dirty={dirty} />
      <StageNav stage={stage} setStage={go} d={d} s={s} compact />

      <div className="flex min-h-0 flex-1 flex-col lg:flex-row">
        <StageNav stage={stage} setStage={go} d={d} s={s} />

        <div className="flex min-h-0 min-w-0 flex-1 flex-col lg:flex-row">
          <main ref={scroller} className="min-w-0 flex-1 lg:overflow-y-auto">
            <div className="on-dark relative isolate overflow-hidden border-b border-rule">
              <img
                src="/images/shar-hero.webp"
                alt=""
                className="absolute inset-0 -z-20 size-full object-cover object-[50%_20%] opacity-80"
              />
              <div className="absolute inset-0 -z-10 bg-linear-to-r from-[oklch(0.15_0.012_285/0.85)] via-[oklch(0.15_0.012_285/0.4)] to-[oklch(0.15_0.012_285/0.1)]" />
              <div className="mx-auto max-w-3xl px-5 py-9 sm:px-8">
                <p className="eyebrow text-gold">
                  Stage {idx + 1} of {stages.length}
                </p>
                <h1 className="display mt-4 text-[clamp(2.25rem,4vw,3.25rem)] leading-[0.98]">
                  {current.label}
                </h1>
                <p className="mt-3 text-sm text-mute">{current.hint}</p>
              </div>
            </div>

            <div className="mx-auto max-w-3xl px-5 py-9 sm:px-8">
              <View {...props} />
              <div className="mt-14 flex items-center justify-between border-t border-rule pt-6">
                <button
                  type="button"
                  disabled={idx === 0}
                  onClick={() => go(stages[idx - 1].id)}
                  className="btn-line rounded-sm px-5 py-3 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Back
                </button>
                <button
                  type="button"
                  disabled={idx === stages.length - 1}
                  onClick={() => go(stages[idx + 1].id)}
                  className="btn-solid rounded-sm px-5 py-3 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Next: {stages[Math.min(idx + 1, stages.length - 1)].label}
                </button>
              </div>
            </div>
          </main>

          <aside className="min-w-0 border-t border-rule bg-deep lg:w-[24rem] lg:shrink-0 lg:overflow-y-auto lg:border-t-0 lg:border-l xl:w-[26rem]">
            <div
              role="tablist"
              aria-label="Detail panel"
              className="sticky top-0 z-10 flex border-b border-rule bg-deep"
            >
              {(["description", "sheet"] as const).map((t) => (
                <button
                  key={t}
                  type="button"
                  role="tab"
                  aria-selected={tab === t}
                  onClick={() => setTab(t)}
                  className={cn(
                    "display flex-1 border-b-2 px-4 py-4 text-sm tracking-[0.12em] transition-colors",
                    tab === t
                      ? "border-gold text-ink"
                      : "border-transparent text-mute hover:text-ink"
                  )}
                >
                  {t === "description" ? "Description" : "Sheet"}
                </button>
              ))}
            </div>
            <div className="p-5 sm:p-6">
              {tab === "description" ? (
                <DescriptionPanel focus={focus} stage={stage} d={d} />
              ) : (
                <SheetPanel s={s} d={d} />
              )}
            </div>
          </aside>
        </div>
      </div>
    </div>
  )
}
