import { useState, type CSSProperties } from "react"
import { cn } from "ui-framework"

import {
  links,
  nodes,
  statusColor,
  statusHint,
  statusLabel,
  type Node,
  type Status,
} from "../data/content"
import { Aurora } from "./aurora"
import { StatusGlyph } from "./glyphs"
import { Reveal } from "./reveal"
import { containerClass } from "./site-header"
import { Starfield } from "./starfield"

const VIEW_W = 1000
const VIEW_H = 560
const px = (x: number) => (x / 100) * VIEW_W
const py = (y: number) => (y / 100) * VIEW_H

const byId = new Map(nodes.map((n) => [n.id, n]))
const statuses = Object.keys(statusLabel) as Status[]
const isFar = (n: Node) => n.status === "exploring" || n.status === "daydream"

const labelPosition: Record<Node["label"], string> = {
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

const nebula = [
  "radial-gradient(50% 45% at 88% 10%, color-mix(in oklch, var(--l-cool) 6%, transparent), transparent)",
  "radial-gradient(55% 50% at 10% 92%, color-mix(in oklch, var(--l-ink) 4%, transparent), transparent)",
].join(",")

function Links({ highlight }: { highlight: Status | null }) {
  return (
    <>
      {links.map(([a, b], i) => {
        const from = byId.get(a)!
        const to = byId.get(b)!
        const dim =
          highlight !== null &&
          from.status !== highlight &&
          to.status !== highlight

        if (isFar(from) || isFar(to)) {
          const far = isFar(from) ? from : to
          return (
            <g
              key={`${a}-${b}`}
              data-dim={dim}
              className="dimmable chart-fade"
              style={{ "--d": `${500 + i * 160}ms` } as CSSProperties}
            >
              <line
                x1={px(from.x)}
                y1={py(from.y)}
                x2={px(to.x)}
                y2={py(to.y)}
                stroke="var(--l-cool)"
                strokeOpacity={far.status === "daydream" ? 0.3 : 0.5}
                strokeWidth={1.25}
                strokeDasharray="2 7"
                strokeLinecap="round"
              />
            </g>
          )
        }

        const strong = from.status === "building" && to.status === "building"
        return (
          <g key={`${a}-${b}`} data-dim={dim} className="dimmable">
            <line
              className="chart-line"
              pathLength={1}
              x1={px(from.x)}
              y1={py(from.y)}
              x2={px(to.x)}
              y2={py(to.y)}
              stroke={strong ? "var(--l-gold)" : "var(--l-ink)"}
              strokeOpacity={strong ? 0.7 : 0.22}
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

function Chart({
  selected,
  onSelect,
  highlight,
}: {
  selected: string
  onSelect: (id: string) => void
  highlight: Status | null
}) {
  const hub = byId.get("builder")!

  return (
    <div
      className="relative isolate overflow-hidden"
      style={{ backgroundImage: nebula }}
    >
      <Starfield count={170} seed={21} />
      <Aurora />

      <div
        className="relative mx-auto w-full"
        style={{ aspectRatio: `${VIEW_W} / ${VIEW_H}` }}
      >
        <svg
          viewBox={`0 0 ${VIEW_W} ${VIEW_H}`}
          aria-hidden="true"
          className="absolute inset-0 size-full overflow-visible text-ink"
        >
          {Array.from({ length: 21 }, (_, i) => (
            <g key={i} stroke="currentColor" strokeOpacity="0.22">
              <line x1={i * 50} y1={0} x2={i * 50} y2={i % 5 === 0 ? 11 : 5} />
              <line
                x1={i * 50}
                y1={VIEW_H}
                x2={i * 50}
                y2={VIEW_H - (i % 5 === 0 ? 11 : 5)}
              />
            </g>
          ))}
          {[150, 300, 470, 650].map((r) => (
            <circle
              key={r}
              cx={px(hub.x)}
              cy={py(hub.y)}
              r={r}
              fill="none"
              stroke="currentColor"
              strokeOpacity="0.09"
              strokeDasharray="2 8"
            />
          ))}
          <Links highlight={highlight} />
        </svg>

        {nodes.map((node) => {
          const active = node.id === selected
          const color = statusColor[node.status]
          const dim = highlight !== null && node.status !== highlight
          return (
            <button
              key={node.id}
              type="button"
              aria-pressed={active}
              aria-label={`${node.name}, ${node.kind.toLowerCase()}, ${statusLabel[node.status].toLowerCase()}`}
              onClick={() => onSelect(node.id)}
              onMouseEnter={() => onSelect(node.id)}
              onFocus={() => onSelect(node.id)}
              data-dim={dim}
              className="dimmable group absolute -translate-x-1/2 -translate-y-1/2 rounded-full p-3"
              style={{ left: `${node.x}%`, top: `${node.y}%` }}
            >
              <span className="relative grid place-items-center">
                {node.status === "building" && (
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
                  status={node.status}
                  className={cn(
                    "relative motion-safe:transition-transform motion-safe:duration-300 motion-safe:group-hover:scale-125",
                    markerSize[node.status]
                  )}
                />
              </span>
              <span
                className={cn(
                  "absolute w-max max-w-44 leading-tight",
                  labelPosition[node.label]
                )}
              >
                <span
                  className="eyebrow mb-1.5 block"
                  style={{ color: active ? color : "var(--l-mute)" }}
                >
                  {node.kind}
                </span>
                <span
                  className={cn(
                    "display block text-base tracking-wider transition-colors sm:text-lg",
                    active ? "text-ink" : "text-ink/75"
                  )}
                >
                  {node.name}
                </span>
              </span>
            </button>
          )
        })}
      </div>
    </div>
  )
}

function Legend({
  highlight,
  pinned,
  onHover,
  onPin,
}: {
  highlight: Status | null
  pinned: Status | null
  onHover: (status: Status | null) => void
  onPin: (status: Status) => void
}) {
  return (
    <ul className="grid grid-cols-2 gap-2.5">
      {statuses.map((status) => {
        const count = nodes.filter((n) => n.status === status).length
        const color = statusColor[status]
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
              className="group flex w-full items-center gap-3 rounded-xl border bg-white/[0.03] px-3.5 py-3 text-left transition-[border-color,background-color,box-shadow] duration-300"
              style={{
                borderColor: on
                  ? color
                  : "color-mix(in oklch, var(--l-ink) 10%, transparent)",
                backgroundColor: on
                  ? "color-mix(in oklch, var(--l-ink) 6%, transparent)"
                  : undefined,
              }}
            >
              <StatusGlyph status={status} className="size-6 shrink-0" />
              <span className="min-w-0">
                <span className="display block text-sm tracking-wider">
                  {statusLabel[status]}
                </span>
                <span className="mt-1 block truncate text-xs text-mute">
                  {statusHint[status]}
                </span>
              </span>
              <span className="display ml-auto text-lg text-mute tabular-nums">
                {count}
              </span>
            </button>
          </li>
        )
      })}
    </ul>
  )
}

function Detail({ node }: { node: Node }) {
  const color = statusColor[node.status]
  const neighbours = links
    .filter(([a, b]) => a === node.id || b === node.id)
    .map(([a, b]) => byId.get(a === node.id ? b : a)!)

  return (
    <div className="relative pl-5">
      <span
        className="absolute inset-y-0 left-0 w-0.5 rounded-full"
        style={{ background: color }}
      />
      <p className="eyebrow" style={{ color }}>
        {node.kind} &middot; {statusLabel[node.status]}
      </p>
      <p className="display mt-3 text-3xl tracking-wide">{node.name}</p>
      <p className="mt-3 max-w-prose text-sm leading-relaxed text-mute">
        {node.summary}
      </p>
      {neighbours.length > 0 && (
        <p className="mt-4 text-xs text-mute">
          <span className="eyebrow mr-2">Linked with</span>
          {neighbours.map((n) => n.name).join(" \u00b7 ")}
        </p>
      )}
    </div>
  )
}

function MobileList() {
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
                    <h3 className="display text-xl tracking-wide">{n.name}</h3>
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

export function SkySection() {
  const [selected, setSelected] = useState("builder")
  const [hovered, setHovered] = useState<Status | null>(null)
  const [pinned, setPinned] = useState<Status | null>(null)
  const highlight = hovered ?? pinned
  const node = byId.get(selected)!

  const legend = (
    <Legend
      highlight={highlight}
      pinned={pinned}
      onHover={setHovered}
      onPin={(s) => setPinned((p) => (p === s ? null : s))}
    />
  )

  return (
    <section id="sky" className="relative isolate border-t border-rule">
      <div className={`${containerClass} py-20 sm:py-32`}>
        <Reveal className="max-w-3xl">
          <p className="eyebrow text-gold">The star map</p>
          <h2 className="display mt-5 text-5xl leading-[0.95] sm:text-7xl">
            One builder today.{" "}
            <span className="block w-fit text-gold">A whole sky later.</span>
          </h2>
          <p className="mt-6 max-w-2xl leading-relaxed text-mute">
            Starlights is built as separate modules on a shared platform, so new
            tools and new games can arrive one at a time without disturbing what
            already works. Nothing past the builder is promised, only intended.
          </p>
        </Reveal>

        <Reveal className="mt-14 hidden md:block" delay={100}>
          <div className="overflow-hidden rounded-2xl border border-rule bg-deep shadow-[0_40px_100px_-50px_rgb(0_0_0/0.9)]">
            <Chart
              selected={selected}
              onSelect={setSelected}
              highlight={highlight}
            />
            <div className="grid gap-8 border-t border-rule bg-sky/60 p-6 backdrop-blur-sm md:grid-cols-12 md:p-8">
              <div className="md:col-span-6">
                <Detail node={node} />
              </div>
              <div className="md:col-span-6">{legend}</div>
            </div>
          </div>
        </Reveal>

        <div className="mt-12 md:hidden">
          <div className="mb-8">{legend}</div>
          <MobileList />
        </div>
      </div>
    </section>
  )
}
