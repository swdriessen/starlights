import type { ReactNode } from "react"
import { cn } from "ui-framework"

import { StarMark } from "../../components/glyphs"
import type { StageStatus } from "./compute"

export function SectionTitle({
  title,
  hint,
  aside,
}: {
  title: string
  hint?: string
  aside?: ReactNode
}) {
  return (
    <div className="flex items-end justify-between gap-4 border-b border-rule pb-3">
      <div>
        <h3 className="display text-xl tracking-wide">{title}</h3>
        {hint && <p className="mt-1.5 text-sm text-mute">{hint}</p>}
      </div>
      {aside}
    </div>
  )
}

export function Chip({
  on,
  onClick,
  disabled,
  warn,
  title,
  children,
  onHover,
}: {
  on?: boolean
  onClick?: () => void
  disabled?: boolean
  warn?: boolean
  title?: string
  children: ReactNode
  onHover?: (over: boolean) => void
}) {
  return (
    <button
      type="button"
      aria-pressed={on}
      disabled={disabled}
      title={title}
      onClick={onClick}
      onMouseEnter={() => onHover?.(true)}
      onMouseLeave={() => onHover?.(false)}
      onFocus={() => onHover?.(true)}
      onBlur={() => onHover?.(false)}
      className={cn(
        "rounded-sm border px-3 py-1.5 text-[0.8125rem] transition-colors",
        on
          ? "border-gold bg-gold/12 text-ink"
          : "border-rule text-mute hover:border-ink/35 hover:text-ink",
        warn && "border-dashed border-gold/70",
        disabled &&
          "cursor-not-allowed opacity-45 hover:border-rule hover:text-mute"
      )}
    >
      {children}
    </button>
  )
}

export function OptionCard({
  on,
  onClick,
  onHover,
  children,
}: {
  on: boolean
  onClick: () => void
  onHover: (over: boolean) => void
  children: ReactNode
}) {
  return (
    <button
      type="button"
      aria-pressed={on}
      onClick={onClick}
      onMouseEnter={() => onHover(true)}
      onMouseLeave={() => onHover(false)}
      onFocus={() => onHover(true)}
      onBlur={() => onHover(false)}
      className={cn(
        "group relative w-full rounded-sm border p-4 text-left transition-colors",
        on
          ? "border-gold bg-gold/[0.07]"
          : "border-rule hover:border-ink/35 hover:bg-ink/[0.03]"
      )}
    >
      {on && <StarMark className="absolute top-3.5 right-3.5 size-3.5" />}
      {children}
    </button>
  )
}

export function Tag({ children }: { children: ReactNode }) {
  return (
    <span className="rounded-sm bg-ink/[0.07] px-2 py-0.5 text-[0.6875rem] text-mute">
      {children}
    </span>
  )
}

/** Stage status mark: filled star = complete, ring = still to do, dashed = optional, cross = conflict. */
export function StageIcon({ state }: { state: StageStatus["state"] }) {
  const color = state === "blocked" ? "var(--l-ink)" : "var(--l-gold)"
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" className="size-4 shrink-0">
      {(state === "complete" || state === "ready") && (
        <path
          d="M12 1C12.8 7.5 16.5 11.2 23 12C16.5 12.8 12.8 16.5 12 23C11.2 16.5 7.5 12.8 1 12C7.5 11.2 11.2 7.5 12 1Z"
          fill={color}
        />
      )}
      {state === "incomplete" && (
        <path
          d="M12 1C12.8 7.5 16.5 11.2 23 12C16.5 12.8 12.8 16.5 12 23C11.2 16.5 7.5 12.8 1 12C7.5 11.2 11.2 7.5 12 1Z"
          fill="none"
          stroke={color}
          strokeWidth="1.6"
        />
      )}
      {state === "optional" && (
        <circle
          cx="12"
          cy="12"
          r="7"
          fill="none"
          stroke={color}
          strokeWidth="1.6"
          strokeDasharray="2.5 3"
        />
      )}
      {state === "blocked" && (
        <g stroke={color} strokeWidth="2" strokeLinecap="round">
          <path d="M12 3v11" />
          <circle cx="12" cy="19.5" r="0.6" fill={color} />
        </g>
      )}
    </svg>
  )
}

export function Notice({
  tone = "info",
  title,
  children,
}: {
  tone?: "info" | "warn"
  title: string
  children: ReactNode
}) {
  return (
    <div
      role={tone === "warn" ? "alert" : "note"}
      className={cn(
        "rounded-sm border px-4 py-3 text-sm",
        tone === "warn"
          ? "border-gold/60 bg-gold/[0.07]"
          : "border-rule bg-ink/[0.03]"
      )}
    >
      <p className="eyebrow mb-2 text-gold">{title}</p>
      <div className="leading-relaxed text-mute">{children}</div>
    </div>
  )
}
