import { cn } from "ui-framework"

import { statusColor, type Status } from "../data/content"

const STAR_PATH =
  "M12 1C12.8 7.5 16.5 11.2 23 12C16.5 12.8 12.8 16.5 12 23C11.2 16.5 7.5 12.8 1 12C7.5 11.2 11.2 7.5 12 1Z"

/** The Starlights mark: a four-pointed star. */
export function StarMark({
  className,
  color = "var(--l-gold)",
}: {
  className?: string
  color?: string
}) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" className={className}>
      <path d={STAR_PATH} fill={color} />
    </svg>
  )
}

/** A star whose rendering encodes roadmap status; animated unless motion is reduced. */
export function StatusGlyph({
  status,
  className,
}: {
  status: Status
  className?: string
}) {
  const color = statusColor[status]

  return (
    <svg
      viewBox="-4 -4 32 32"
      aria-hidden="true"
      className={cn("overflow-visible", className)}
    >
      {status === "building" && (
        <path
          d={STAR_PATH}
          fill={color}
          style={{ filter: `drop-shadow(0 0 5px ${color})` }}
        />
      )}
      {status === "planned" && (
        <path d={STAR_PATH} fill={color} className="breathe" />
      )}
      {status === "exploring" && (
        <>
          <path
            d={STAR_PATH}
            fill="none"
            stroke={color}
            strokeWidth="1.6"
            strokeLinejoin="round"
          />
          <circle
            className="orbit"
            cx="12"
            cy="12"
            r="14"
            fill="none"
            stroke={color}
            strokeOpacity="0.8"
            strokeWidth="1.2"
            strokeDasharray="2 5"
            strokeLinecap="round"
          />
        </>
      )}
      {status === "daydream" && (
        <g className="flicker">
          <circle cx="12" cy="12" r="2.4" fill={color} />
          <circle
            cx="12"
            cy="12"
            r="8"
            fill="none"
            stroke={color}
            strokeOpacity="0.7"
            strokeWidth="1.4"
            strokeDasharray="1.5 4.5"
            strokeLinecap="round"
          />
        </g>
      )}
    </svg>
  )
}
