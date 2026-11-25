import type { CSSProperties } from "react"

type Band = {
  top: string
  height: string
  left: string
  width: string
  rotate: string
  c1: string
  c2: string
  duration: string
  delay: string
}

const bands: Band[] = [
  {
    top: "20%",
    height: "22%",
    left: "-10%",
    width: "95%",
    rotate: "-12deg",
    c1: "var(--l-cool)",
    c2: "var(--l-gold)",
    duration: "38s",
    delay: "0s",
  },
  {
    top: "54%",
    height: "18%",
    left: "20%",
    width: "90%",
    rotate: "-7deg",
    c1: "var(--l-cool)",
    c2: "var(--l-cool)",
    duration: "46s",
    delay: "-14s",
  },
]

/** Faint aurora streaks drifting through the star map. Decorative only. */
export function Aurora() {
  return (
    <div
      aria-hidden="true"
      className="pointer-events-none absolute inset-0 -z-10 overflow-hidden"
    >
      {bands.map((band, i) => (
        <div
          key={i}
          className="aurora-band"
          style={
            {
              top: band.top,
              height: band.height,
              left: band.left,
              width: band.width,
              "--r": band.rotate,
              "--c1": band.c1,
              "--c2": band.c2,
              "--t": band.duration,
              "--d": band.delay,
            } as CSSProperties
          }
        />
      ))}
    </div>
  )
}
