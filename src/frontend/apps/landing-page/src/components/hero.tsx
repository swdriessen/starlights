import { ArrowRight } from "lucide-react"
import type { CSSProperties } from "react"
import { cn } from "ui-framework"

import {
  builderCta,
  builderHref,
  statusLabel,
  tiles,
  type Tile,
} from "../data/content"
import { containerClass } from "./site-header"
import { Starfield } from "./starfield"

function PlateTile({
  tile,
  className,
  featured = false,
  delay,
}: {
  tile: Tile
  className?: string
  featured?: boolean
  delay: number
}) {
  const live = tile.status === "building"

  const body = (
    <>
      <div className="absolute inset-0 overflow-hidden">
        <div className="size-full ease-out motion-safe:transition-transform motion-safe:duration-1000 motion-safe:group-hover:scale-[1.03]">
          <img
            src={tile.image}
            alt={tile.alt}
            className="settle size-full object-cover saturate-[0.8] transition-[filter] duration-700 group-hover:saturate-100"
            style={{ objectPosition: tile.position }}
            loading="eager"
            fetchPriority={featured ? "high" : "auto"}
            decoding="async"
          />
        </div>
      </div>
      <div className="absolute inset-0 bg-linear-to-t from-black/85 via-black/30 to-black/5" />

      <div className="absolute top-5 right-5 left-5 flex items-start justify-between gap-3 text-white">
        <span className="eyebrow inline-flex items-center gap-2 rounded-full bg-black/50 py-2 pr-3.5 pl-3 backdrop-blur-sm">
          <span
            className={cn(
              "size-1.5 rounded-full",
              live ? "bg-gold" : "bg-white/60"
            )}
          />
          {statusLabel[tile.status]}
        </span>
        <span className="display text-xl font-light tracking-widest text-white/60">
          {tile.plate}
        </span>
      </div>

      <div
        className={cn(
          "absolute right-5 bottom-5 left-5 text-white",
          featured && "sm:right-9 sm:bottom-9 sm:left-9"
        )}
      >
        {tile.tag && <p className="eyebrow mb-3 text-gold">{tile.tag}</p>}
        <h2
          className={cn(
            "display tracking-wide",
            featured ? "text-4xl sm:text-6xl" : "text-2xl sm:text-3xl"
          )}
        >
          {tile.title}
        </h2>
        <p
          className={cn(
            "mt-3 max-w-md leading-relaxed text-white/85",
            featured ? "text-base" : "text-sm"
          )}
        >
          {tile.blurb}
        </p>
        {live && (
          <p className="mt-5 inline-flex items-center gap-2 border-b border-gold pb-1 text-[0.8125rem] font-semibold tracking-wider text-gold uppercase transition-[gap] group-hover:gap-3">
            {builderCta} <ArrowRight className="size-4" aria-hidden="true" />
          </p>
        )}
      </div>
    </>
  )

  const shared = cn(
    "group rise relative isolate block overflow-hidden rounded-xl border-4 border-double border-ink/30 bg-deep shadow-[0_30px_60px_-30px_rgb(0_0_0/0.6)]",
    className
  )
  const style = { "--d": `${delay}ms` } as CSSProperties

  return live ? (
    <a href={builderHref} className={shared} style={style}>
      {body}
    </a>
  ) : (
    <article className={shared} style={style}>
      {body}
    </article>
  )
}

export function Hero() {
  const [builder, campaigns, compendium] = tiles

  return (
    <section id="top" className="relative isolate overflow-hidden">
      <Starfield
        count={70}
        className="[mask-image:linear-gradient(to_bottom,black_0%,black_30%,transparent_80%)]"
      />

      <div className={`${containerClass} pt-14 pb-16 sm:pt-24 sm:pb-24`}>
        <div className="mx-auto max-w-4xl text-center">
          <p className="eyebrow rise mb-7 text-gold">
            Project Starlights &middot; early development
          </p>
          <h1
            className="display rise text-[clamp(3.25rem,9vw,7.5rem)] leading-[0.92] tracking-tight"
            style={{ "--d": "80ms" } as CSSProperties}
          >
            Heroes <span className="text-gold">first.</span>
            <span className="mt-5 block text-[clamp(1.05rem,2.3vw,1.7rem)] leading-tight font-normal tracking-[0.12em] text-mute sm:mt-6">
              The rest of the table follows.
            </span>
          </h1>
        </div>

        <div className="mt-14 grid gap-4 sm:mt-20 md:h-[36rem] md:grid-cols-5 md:grid-rows-2">
          <PlateTile
            tile={builder}
            featured
            delay={260}
            className="min-h-[28rem] md:col-span-3 md:row-span-2"
          />
          <PlateTile
            tile={campaigns}
            delay={380}
            className="min-h-[16rem] md:col-span-2"
          />
          <PlateTile
            tile={compendium}
            delay={500}
            className="min-h-[16rem] md:col-span-2"
          />
        </div>
      </div>
    </section>
  )
}
