import { ArrowUpRight } from "lucide-react"
import type { CSSProperties } from "react"

import { StarMark } from "../../components/glyphs"
import { Reveal } from "../../components/reveal"
import {
  builderCta,
  builderHref,
  REPO_URL,
  statusLabel,
  tiles,
} from "../../data/content"
import { artNote, points, trademarkNote } from "../copy"
import { Sky, type Layout } from "../sky"
import { VariantLinks } from "../variant-links"
import "./v1.css"

const container = "mx-auto w-full max-w-[1180px] px-5 sm:px-8"
const roman = ["I", "II", "III"]

/** Astrolabe layout: hub at the centre, tools on the inner ring, rulesets beyond. */
const layout: Layout = {
  builder: { x: 50, y: 50, label: "bottom" },
  compendium: { x: 33, y: 28, label: "top" },
  campaigns: { x: 63, y: 24, label: "top" },
  homebrew: { x: 29, y: 72, label: "bottom" },
  dnd5e: { x: 72, y: 52, label: "bottom" },
  daggerheart: { x: 88, y: 28, label: "bottom" },
  pathfinder: { x: 86, y: 78, label: "top" },
  dnd4e: { x: 56, y: 86, label: "top" },
  more: { x: 93, y: 8, label: "bottom" },
}

function Astrolabe() {
  const cx = 500
  const cy = 320
  return (
    <g stroke="currentColor" fill="none">
      {[110, 190, 270].map((r) => (
        <circle key={r} cx={cx} cy={cy} r={r} strokeOpacity="0.3" />
      ))}
      <circle cx={cx} cy={cy} r={300} strokeOpacity="0.5" strokeWidth="1.4" />
      <circle cx={cx} cy={cy} r={308} strokeOpacity="0.3" />
      {Array.from({ length: 72 }, (_, i) => {
        const a = (i * 5 * Math.PI) / 180
        const long = i % 6 === 0
        const r1 = 308
        const r2 = long ? 324 : 316
        return (
          <line
            key={i}
            x1={cx + Math.cos(a) * r1}
            y1={cy + Math.sin(a) * r1}
            x2={cx + Math.cos(a) * r2}
            y2={cy + Math.sin(a) * r2}
            strokeOpacity={long ? 0.6 : 0.3}
          />
        )
      })}
      <line x1={cx} y1={0} x2={cx} y2={640} strokeOpacity="0.14" />
      <line x1={0} y1={cy} x2={1000} y2={cy} strokeOpacity="0.14" />
      <line x1={140} y1={20} x2={860} y2={620} strokeOpacity="0.1" />
      <line x1={860} y1={20} x2={140} y2={620} strokeOpacity="0.1" />
    </g>
  )
}

function Header() {
  return (
    <header className="border-b-4 border-double border-ink/40">
      <div className={`${container} flex flex-col items-center gap-3 py-5`}>
        <a
          href="#top"
          className="display flex items-center gap-4 text-2xl tracking-[0.3em] sm:text-3xl"
        >
          <StarMark className="size-5" color="var(--v1-gilt)" />
          Starlights
          <StarMark className="size-5" color="var(--v1-gilt)" />
        </a>
        <nav
          aria-label="Primary"
          className="eyebrow flex flex-wrap justify-center gap-x-8 gap-y-2 text-mute"
        >
          <a className="hover:text-ink" href="#chapters">
            Chapters
          </a>
          <a className="hover:text-ink" href="#builder">
            The Builder
          </a>
          <a className="hover:text-ink" href="#sky">
            Celestial Chart
          </a>
          <a
            className="hover:text-ink"
            href={REPO_URL}
            target="_blank"
            rel="noreferrer"
          >
            GitHub
          </a>
        </nav>
      </div>
    </header>
  )
}

function Hero() {
  return (
    <section
      id="top"
      className={`${container} grid items-center gap-12 py-14 lg:grid-cols-12 lg:py-20`}
    >
      <div className="lg:col-span-5">
        <p className="eyebrow rise text-gold">
          Liber primus &middot; an early draft
        </p>
        <h1
          className="display rise mt-6 text-5xl sm:text-6xl"
          style={{ "--d": "80ms" } as CSSProperties}
        >
          A codex for <span className="text-gold">heroes.</span>
        </h1>
        <p
          className="rise mt-7 leading-relaxed text-mute italic"
          style={{ "--d": "180ms" } as CSSProperties}
        >
          Starlights is a character builder for tabletop roleplaying games,
          beginning with the D&amp;D 5e SRD (5.2.1) &mdash; and the first page
          of a much longer book.
        </p>
        <div
          className="rise mt-9 flex flex-wrap items-center gap-x-8 gap-y-4"
          style={{ "--d": "280ms" } as CSSProperties}
        >
          <a
            href={builderHref}
            className="gilded display px-7 py-3 text-sm tracking-widest"
          >
            {builderCta}
          </a>
          <a
            href="#sky"
            className="eyebrow border-b border-ink/50 pb-1 hover:text-gold"
          >
            Consult the chart
          </a>
        </div>
      </div>

      <figure
        className="rise lg:col-span-7"
        style={{ "--d": "200ms" } as CSSProperties}
      >
        <div className="frame p-2">
          <img
            src="/images/codex-party.webp"
            alt="An illuminated manuscript page: a knight with a lion shield, a robed wizard holding a star and a hooded archer stand under a gothic arch on gold leaf."
            className="aspect-[3/2] w-full object-cover"
            fetchPriority="high"
            decoding="async"
          />
        </div>
        <figcaption className="mt-3 text-center text-sm text-mute italic">
          Folio I &mdash; the fellowship assembled.
        </figcaption>
      </figure>
    </section>
  )
}

function Chapters() {
  return (
    <section id="chapters" className="border-t-4 border-double border-ink/40">
      <div className={`${container} py-16 sm:py-24`}>
        <Reveal className="text-center">
          <p className="eyebrow text-gold">Table of contents</p>
          <h2 className="display mt-4 text-4xl sm:text-5xl">Three chapters</h2>
        </Reveal>

        <div className="mt-14 grid gap-12 md:grid-cols-3 md:gap-8">
          {tiles.map((tile, i) => (
            <Reveal key={tile.id} delay={i * 120} className="text-center">
              <div className="frame mx-auto aspect-[3/4] max-w-xs overflow-hidden rounded-t-[999px] p-1.5">
                <img
                  src={tile.image}
                  alt={tile.alt}
                  loading="lazy"
                  decoding="async"
                  className="size-full rounded-t-[999px] object-cover sepia-[0.25]"
                  style={{ objectPosition: tile.position }}
                />
              </div>
              <p className="uncial mt-6 text-5xl text-gold">{roman[i]}</p>
              <h3 className="display mt-2 text-2xl">{tile.title}</h3>
              {tile.tag && <p className="eyebrow mt-2 text-mute">{tile.tag}</p>}
              <p className="mx-auto mt-4 max-w-xs leading-relaxed text-mute italic">
                {tile.blurb}
              </p>
              <p className="eyebrow mt-5 text-gold">
                &mdash; {statusLabel[tile.status]} &mdash;
              </p>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

function Builder() {
  return (
    <section id="builder" className="border-t-4 border-double border-ink/40">
      <div
        className={`${container} grid items-center gap-14 py-16 sm:py-24 lg:grid-cols-12`}
      >
        <Reveal className="lg:col-span-5">
          <figure className="mx-auto max-w-sm">
            <div className="frame p-2">
              <img
                src="/images/sheet.webp"
                alt="A filled-in ranger character sheet on a dark wooden table, with candlelight, dice, a pencil and a ranger miniature."
                loading="lazy"
                decoding="async"
                className="aspect-[2/3] w-full object-cover sepia-[0.2]"
              />
            </div>
            <figcaption className="mt-3 text-center text-sm text-mute italic">
              Plate IV &mdash; a sheet such as the builder will keep.
            </figcaption>
          </figure>
        </Reveal>

        <Reveal className="lg:col-span-7" delay={120}>
          <p className="eyebrow text-gold">The builder</p>
          <h2 className="display mt-4 text-4xl sm:text-5xl">
            A sheet that <span className="text-gold">shows its work.</span>
          </h2>
          <p className="mt-6 leading-relaxed text-mute italic">
            Most character builders hand you a finished number. Starlights keeps
            the reasoning attached, so you always know where a modifier came
            from and what a choice will cost you later.
          </p>
          <ol className="mt-9 divide-y divide-ink/20 border-y border-ink/30">
            {points.map((point, i) => (
              <li key={point.title} className="flex gap-5 py-5">
                <span className="uncial w-10 shrink-0 text-center text-4xl leading-none text-gold">
                  {roman[i] ?? "IV"}
                </span>
                <div>
                  <h3 className="display text-lg">{point.title}</h3>
                  <p className="mt-1.5 leading-relaxed text-mute">
                    {point.body}
                  </p>
                </div>
              </li>
            ))}
          </ol>
          <p className="mt-5 text-sm text-mute italic">
            First-release targets, not shipped features.
          </p>
        </Reveal>
      </div>
    </section>
  )
}

function SkySection() {
  return (
    <section id="sky" className="border-t-4 border-double border-ink/40">
      <div className={`${container} py-16 sm:py-24`}>
        <Reveal className="mx-auto max-w-2xl text-center">
          <p className="eyebrow text-gold">The celestial chart</p>
          <h2 className="display mt-4 text-4xl sm:text-5xl">
            One builder today.{" "}
            <span className="text-gold">A whole sky later.</span>
          </h2>
          <p className="mt-6 leading-relaxed text-mute italic">
            Starlights is built as separate modules on a shared platform, so new
            tools and new games can arrive one at a time. Nothing past the
            builder is promised, only intended.
          </p>
        </Reveal>

        <div className="mt-12">
          <Sky
            layout={layout}
            size={[1000, 640]}
            decor={<Astrolabe />}
            stars={50}
            frameClass="frame"
            chartClass="bg-deep"
            nameClass="display text-[0.8rem] tracking-wide sm:text-sm"
            titleClass="display"
            lineStrong="var(--l-gold)"
            lineNear="var(--l-ink)"
            lineFar="var(--l-cool)"
            legendClass="rounded-sm"
          />
        </div>
      </div>
    </section>
  )
}

function Follow() {
  return (
    <section className="border-t-4 border-double border-ink/40">
      <Reveal className={`${container} py-16 text-center sm:py-20`}>
        <StarMark className="mx-auto size-6" color="var(--v1-gilt)" />
        <h2 className="display mx-auto mt-5 max-w-3xl text-3xl sm:text-4xl">
          Written in the open,{" "}
          <span className="text-gold">one page at a time.</span>
        </h2>
        <p className="mx-auto mt-5 max-w-xl leading-relaxed text-mute italic">
          Project Starlights is MIT-licensed. A star on GitHub is the kindest
          way to say it is worth continuing.
        </p>
        <a
          href={REPO_URL}
          target="_blank"
          rel="noreferrer"
          className="gilded display mt-8 inline-flex items-center gap-2 px-7 py-3 text-sm tracking-widest"
        >
          Star it on GitHub{" "}
          <ArrowUpRight className="size-4" aria-hidden="true" />
        </a>
      </Reveal>
    </section>
  )
}

function Footer() {
  return (
    <footer className="border-t-4 border-double border-ink/40">
      <div
        className={`${container} grid gap-8 py-10 text-sm text-mute md:grid-cols-12`}
      >
        <div className="md:col-span-4">
          <p className="display flex items-center gap-3 text-lg text-ink">
            <StarMark className="size-4" color="var(--v1-gilt)" /> Starlights
          </p>
          <VariantLinks
            current="v1"
            className="mt-4"
            linkClass="hover:text-ink"
            activeClass="text-gold underline underline-offset-4"
          />
        </div>
        <p className="md:col-span-4">{artNote}</p>
        <p className="md:col-span-4">{trademarkNote}</p>
      </div>
    </footer>
  )
}

export default function V1() {
  return (
    <div className="v1">
      <Header />
      <main>
        <Hero />
        <Chapters />
        <Builder />
        <SkySection />
        <Follow />
      </main>
      <Footer />
    </div>
  )
}
