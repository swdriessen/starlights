import { useState, type CSSProperties, type ReactNode } from "react"
import { cn } from "ui-framework"

import { StatusGlyph } from "../components/glyphs"
import { Reveal } from "../components/reveal"
import { Starfield } from "../components/starfield"
import {
  links,
  nodes,
  statusColor,
  statusHint,
  statusLabel,
  type Node,
  type Status,
} from "../data/content"

export type Place = {
  /** Position in percent of the chart. */
  x: number
  y: number
  label: "top" | "bottom" | "left" | "right"
}
export type Layout = Record<string, Place>

type SkyProps = {
  layout: Layout
  /** SVG user-space size; also fixes the chart aspect ratio. */
  size?: [number, number]
  /** Decorative SVG drawn behind the links (rings, roads, hills...). */
  decor?: ReactNode
  stars?: number
  seed?: number
  /** Classes for the chart surface (background etc). */
  chartClass?: string
  /** Outer frame classes. */
  frameClass?: string
  /** Font/size classes for node names, e.g. a display family. */
  nameClass?: string
  /** Classes for the small "Tool / Ruleset" caption above node names. */
  kindClass?: string
  /** Display font class for detail title and legend titles. */
  titleClass?: string
  /** Colour of lines between built stars, and of lines toward unconfirmed goals. */
  lineStrong?: string
  lineNear?: string
  lineFar?: string
  /** Bend of the lines, as a fraction of their length. 0 is straight. */
  bend?: number
  legendClass?: string
  detailClass?: string
  /** Sidebar arrangement instead of detail+legend below the chart. */
  side?: boolean
  mobileHeading?: string
}

const byId = new Map(nodes.map((n) => [n.id, n]))
const statuses = Object.keys(statusLabel) as Status[]
const isFar = (n: Node) => n.status === "exploring" || n.status === "daydream"

const labelPosition: Record<Place["label"], string> = {
  bottom: "top-full mt-2 left-1/2 -translate-x-1/2 text-center",
  top: "bottom-full mb-2 left-1/2 -translate-x-1/2 text-center",
  left: "right-full mr-3 top-1/2 -translate-y-1/2 text-right",
  right: "left-full ml-3 top-1/2 -translate-y-1/2 text-left",
}

const markerSize: Record<Status, string> = {
  building: "size-8",
  planned: "size-6",
  exploring: "size-6",
  daydream: "size-6",
}

function Links({
  layout,
  size,
  highlight,
  lineStrong,
  lineNear,
  lineFar,
  bend,
}: {
  layout: Layout
  size: [number, number]
  highlight: Status | null
  lineStrong: string
  lineNear: string
  lineFar: string
  bend: number
}) {
  const px = (x: number) => (x / 100) * size[0]
  const py = (y: number) => (y / 100) * size[1]

  return (
    <>
      {links.map(([a, b], i) => {
        const from = byId.get(a)!
        const to = byId.get(b)!
        const p = layout[a]
        const q = layout[b]
        const x1 = px(p.x)
        const y1 = py(p.y)
        const x2 = px(q.x)
        const y2 = py(q.y)
        const dx = x2 - x1
        const dy = y2 - y1
        const len = Math.hypot(dx, dy)
        const sign = i % 2 ? 1 : -1
        const cx = (x1 + x2) / 2 + (-dy / len) * len * bend * sign
        const cy = (y1 + y2) / 2 + (dx / len) * len * bend * sign
        const d = `M${x1} ${y1} Q${cx} ${cy} ${x2} ${y2}`
        const dim =
          highlight !== null &&
          from.status !== highlight &&
          to.status !== highlight
        const far = isFar(from) || isFar(to)
        const strong = from.status === "building" && to.status === "building"

        if (far) {
          const target = isFar(from) ? from : to
          return (
            <g
              key={`${a}-${b}`}
              data-dim={dim}
              className="dimmable chart-fade"
              style={{ "--d": `${500 + i * 160}ms` } as CSSProperties}
            >
              <path
                d={d}
                fill="none"
                stroke={lineFar}
                strokeOpacity={target.status === "daydream" ? 0.4 : 0.7}
                strokeWidth={1.3}
                strokeDasharray="2 7"
                strokeLinecap="round"
              />
            </g>
          )
        }

        return (
          <g key={`${a}-${b}`} data-dim={dim} className="dimmable">
            <path
              d={d}
              fill="none"
              className="chart-line"
              pathLength={1}
              stroke={strong ? lineStrong : lineNear}
              strokeOpacity={strong ? 0.9 : 0.55}
              strokeWidth={1.5}
              strokeLinecap="round"
              style={{ "--d": `${200 + i * 140}ms` } as CSSProperties}
            />
          </g>
        )
      })}
    </>
  )
}

function Legend({
  highlight,
  pinned,
  onHover,
  onPin,
  titleClass,
  legendClass,
  single = false,
}: {
  highlight: Status | null
  pinned: Status | null
  onHover: (status: Status | null) => void
  onPin: (status: Status) => void
  titleClass?: string
  legendClass?: string
  single?: boolean
}) {
  return (
    <ul className={cn("grid gap-2.5", single ? "grid-cols-1" : "grid-cols-2")}>
      {statuses.map((status) => {
        const count = nodes.filter((n) => n.status === status).length
        const on = highlight === status
        return (
          <li key={status}>
            <button
              type="button"
              aria-pressed={pinned === status}
              onMouseEnter={() => onHover(status)}
              onMouseLeave={() => onHover(null)}
              onFocus={() => onHover(status)}
              onBlur={() => onHover(null)}
              onClick={() => onPin(status)}
              data-on={on}
              className={cn(
                "flex w-full items-center gap-3 border px-3.5 py-3 text-left transition-colors duration-300",
                legendClass
              )}
              style={{
                borderColor: on
                  ? statusColor[status]
                  : "color-mix(in oklch, var(--l-ink) 16%, transparent)",
                backgroundColor: on
                  ? "color-mix(in oklch, var(--l-ink) 7%, transparent)"
                  : undefined,
              }}
            >
              <StatusGlyph status={status} className="size-6 shrink-0" />
              <span className="min-w-0 flex-1">
                <span className={cn("block text-sm", titleClass)}>
                  {statusLabel[status]}
                </span>
                <span className="mt-1 block text-xs leading-snug text-mute">
                  {statusHint[status]}
                </span>
              </span>
              <span
                className={cn(
                  "shrink-0 text-lg text-mute tabular-nums",
                  titleClass
                )}
              >
                {count}
              </span>
            </button>
          </li>
        )
      })}
    </ul>
  )
}

function Detail({
  node,
  titleClass,
  detailClass,
}: {
  node: Node
  titleClass?: string
  detailClass?: string
}) {
  const color = statusColor[node.status]
  const neighbours = links
    .filter(([a, b]) => a === node.id || b === node.id)
    .map(([a, b]) => byId.get(a === node.id ? b : a)!)

  return (
    <div className={cn("relative pl-5", detailClass)}>
      <span
        className="absolute inset-y-0 left-0 w-0.5 rounded-full"
        style={{ background: color }}
      />
      <p className="eyebrow" style={{ color }}>
        {node.kind} &middot; {statusLabel[node.status]}
      </p>
      <p className={cn("mt-3 text-3xl", titleClass)}>{node.name}</p>
      <p className="mt-3 max-w-prose leading-relaxed text-mute">
        {node.summary}
      </p>
      {neighbours.length > 0 && (
        <p className="mt-4 text-sm text-mute">
          <span className="eyebrow mr-2">Linked with</span>
          {neighbours.map((n) => n.name).join(" \u00b7 ")}
        </p>
      )}
    </div>
  )
}

/**
 * Every node's detail is rendered in the same grid cell and only the selected one is visible,
 * so the panel is always as tall as the longest entry and the page never jumps on selection.
 */
function DetailStack({
  selected,
  titleClass,
  detailClass,
}: {
  selected: string
  titleClass?: string
  detailClass?: string
}) {
  return (
    <div className="grid">
      {nodes.map((n) => (
        <div
          key={n.id}
          className={cn("[grid-area:1/1]", n.id !== selected && "invisible")}
        >
          <Detail node={n} titleClass={titleClass} detailClass={detailClass} />
        </div>
      ))}
    </div>
  )
}

function MobileList({ titleClass }: { titleClass?: string }) {
  return (
    <div className="grid gap-10 md:hidden">
      {(["Tool", "Ruleset", "Someday"] as const).map((kind) => (
        <div key={kind}>
          <p className="eyebrow mb-1 text-gold">
            {kind === "Tool"
              ? "Tools"
              : kind === "Ruleset"
                ? "Rulesets"
                : "Someday"}
          </p>
          <ul className="divide-y divide-rule border-b border-rule">
            {nodes
              .filter((n) => n.kind === kind)
              .map((n) => (
                <li key={n.id} className="py-4">
                  <div className="flex items-center justify-between gap-3">
                    <h3 className={cn("text-xl", titleClass)}>{n.name}</h3>
                    <span className="inline-flex shrink-0 items-center gap-2 text-xs text-mute">
                      <StatusGlyph status={n.status} className="size-4" />
                      {statusLabel[n.status]}
                    </span>
                  </div>
                  <p className="mt-2 text-sm leading-relaxed text-mute">
                    {n.summary}
                  </p>
                </li>
              ))}
          </ul>
        </div>
      ))}
    </div>
  )
}

/** The roadmap as an interactive constellation; every variant lays it out in its own way. */
export function Sky({
  layout,
  size = [1000, 560],
  decor,
  stars = 120,
  seed = 21,
  chartClass,
  frameClass,
  nameClass,
  kindClass,
  titleClass,
  lineStrong = "var(--l-gold)",
  lineNear = "var(--l-ink)",
  lineFar = "var(--l-cool)",
  bend = 0,
  legendClass,
  detailClass,
  side = false,
}: SkyProps) {
  const [selected, setSelected] = useState("builder")
  const [hovered, setHovered] = useState<Status | null>(null)
  const [pinned, setPinned] = useState<Status | null>(null)
  const highlight = hovered ?? pinned
  const legend = (
    <Legend
      highlight={highlight}
      pinned={pinned}
      onHover={setHovered}
      onPin={(s) => setPinned((p) => (p === s ? null : s))}
      titleClass={titleClass}
      legendClass={legendClass}
    />
  )

  const chart = (
    <div className={cn("relative isolate overflow-hidden", chartClass)}>
      <Starfield count={stars} seed={seed} />
      <div
        className="relative mx-auto w-full"
        style={{ aspectRatio: `${size[0]} / ${size[1]}` }}
      >
        <svg
          viewBox={`0 0 ${size[0]} ${size[1]}`}
          aria-hidden="true"
          className="pointer-events-none absolute inset-0 size-full overflow-visible text-ink"
        >
          {decor}
          <Links
            layout={layout}
            size={size}
            highlight={highlight}
            lineStrong={lineStrong}
            lineNear={lineNear}
            lineFar={lineFar}
            bend={bend}
          />
        </svg>

        {nodes.map((n) => {
          const place = layout[n.id]
          const active = n.id === selected
          const color = statusColor[n.status]
          const dim = highlight !== null && n.status !== highlight
          return (
            <button
              key={n.id}
              type="button"
              aria-pressed={active}
              aria-label={`${n.name}, ${n.kind.toLowerCase()}, ${statusLabel[n.status].toLowerCase()}`}
              onClick={() => setSelected(n.id)}
              onMouseEnter={() => setSelected(n.id)}
              onFocus={() => setSelected(n.id)}
              data-dim={dim}
              className="dimmable group absolute z-10 -translate-x-1/2 -translate-y-1/2 rounded-full p-3"
              style={{ left: `${place.x}%`, top: `${place.y}%` }}
            >
              <span className="relative grid place-items-center">
                {n.status === "building" && (
                  <span
                    className="halo absolute size-8 rounded-full"
                    style={{ background: color, opacity: 0.28 }}
                  />
                )}
                {active && (
                  <span
                    className="absolute size-12 rounded-full border"
                    style={{ borderColor: color }}
                  />
                )}
                <StatusGlyph
                  status={n.status}
                  className={cn(
                    "relative motion-safe:transition-transform motion-safe:duration-300 motion-safe:group-hover:scale-125",
                    markerSize[n.status]
                  )}
                />
              </span>
              <span
                className={cn(
                  "absolute w-max max-w-44 leading-tight",
                  labelPosition[place.label]
                )}
              >
                <span
                  className={cn("eyebrow mb-1.5 block", kindClass)}
                  style={{ color: active ? color : "var(--l-mute)" }}
                >
                  {n.kind}
                </span>
                <span
                  className={cn(
                    "block text-base transition-colors sm:text-lg",
                    nameClass,
                    active ? "text-ink" : "text-ink/75"
                  )}
                >
                  {n.name}
                </span>
              </span>
            </button>
          )
        })}
      </div>
    </div>
  )

  return (
    <>
      <Reveal className="hidden md:block" delay={100}>
        <div className={cn("overflow-hidden", frameClass)}>
          {side ? (
            <div className="grid xl:grid-cols-[1fr_22rem]">
              {chart}
              <div className="grid content-between gap-8 border-t border-rule p-6 xl:border-t-0 xl:border-l">
                <DetailStack
                  selected={selected}
                  titleClass={titleClass}
                  detailClass={detailClass}
                />
                <Legend
                  highlight={highlight}
                  pinned={pinned}
                  onHover={setHovered}
                  onPin={(s) => setPinned((p) => (p === s ? null : s))}
                  titleClass={titleClass}
                  legendClass={legendClass}
                  single
                />
              </div>
            </div>
          ) : (
            <>
              {chart}
              <div className="grid gap-8 border-t border-rule p-6 md:grid-cols-12 md:p-8">
                <div className="md:col-span-6">
                  <DetailStack
                    selected={selected}
                    titleClass={titleClass}
                    detailClass={detailClass}
                  />
                </div>
                <div className="md:col-span-6">{legend}</div>
              </div>
            </>
          )}
        </div>
      </Reveal>

      <div className="md:hidden">
        <div className="mb-8">{legend}</div>
        <MobileList titleClass={titleClass} />
      </div>
    </>
  )
}
