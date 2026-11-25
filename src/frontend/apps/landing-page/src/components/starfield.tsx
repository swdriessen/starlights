import { useMemo, type CSSProperties } from "react"
import { cn } from "ui-framework"

/** Small deterministic PRNG so the sky is identical on every render. */
function mulberry32(seed: number) {
  let a = seed
  return () => {
    a |= 0
    a = (a + 0x6d2b79f5) | 0
    let t = Math.imul(a ^ (a >>> 15), 1 | a)
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296
  }
}

const tints = ["var(--l-ink)", "var(--l-ink)", "var(--l-cool)"]

/**
 * A quiet field of stars: dim specks on the light theme, stars at night.
 * Only a handful twinkle, and none move once reduced motion is requested.
 */
export function Starfield({
  count = 80,
  seed = 7,
  className,
}: {
  count?: number
  seed?: number
  className?: string
}) {
  const stars = useMemo(() => {
    const random = mulberry32(seed)
    return Array.from({ length: count }, (_, i) => ({
      x: random() * 100,
      y: random() * 100,
      size: 1 + random() * 1.8,
      opacity: 0.3 + random() * 0.7,
      tint: tints[Math.floor(random() * tints.length)],
      twinkles: i % 5 === 0,
      duration: 3.5 + random() * 4.5,
      delay: random() * 5,
    }))
  }, [count, seed])

  return (
    <div
      aria-hidden="true"
      className={cn("pointer-events-none absolute inset-0 -z-10", className)}
    >
      {stars.map((star, i) => (
        <span
          key={i}
          className={cn("absolute rounded-full", star.twinkles && "twinkle")}
          style={
            {
              left: `${star.x}%`,
              top: `${star.y}%`,
              width: star.size,
              height: star.size,
              background: star.tint,
              "--o": `calc(${star.opacity.toFixed(2)} * var(--l-dots))`,
              opacity: `calc(${star.opacity.toFixed(2)} * var(--l-dots))`,
              "--t": `${star.duration.toFixed(1)}s`,
              "--d": `${star.delay.toFixed(1)}s`,
            } as CSSProperties
          }
        />
      ))}
    </div>
  )
}
