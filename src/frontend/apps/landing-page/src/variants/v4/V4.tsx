import { ArrowRight, ArrowUpRight } from "lucide-react"
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
import { Armillary } from "../armillary"
import { armillaryLayout } from "../armillary-layout"
import { artNote, points, trademarkNote } from "../copy"
import { Sky } from "../sky"
import { VariantLinks } from "../variant-links"
import "./v4.css"

const container = "mx-auto w-full max-w-[1180px] px-5 sm:px-8"

const [builderTile, campaignTile, compendiumTile] = tiles

const dustColors = [
  "oklch(0.96 0.02 90)",
  "oklch(0.96 0.02 90)",
  "oklch(0.88 0.09 90)",
  "oklch(0.84 0.08 285)",
]

/** Deterministic, so the dust looks the same on every render. */
const dust = Array.from({ length: 46 }, (_, i) => ({
  left: (i * 53 + 11) % 100,
  size: 1.5 + (i % 4) * 0.8,
  t: 13 + (i % 7) * 2.1,
  d: -((i * 1.7) % 20),
  dx: ((i % 9) - 4) * 9,
  o: 0.45 + (i % 4) * 0.17,
  c: dustColors[i % dustColors.length],
}))

function Header() {
  return (
    <header className="on-dark absolute inset-x-0 top-0 z-20">
      <div className={`${container} flex h-20 items-center justify-between`}>
        <a
          href="#top"
          className="display flex items-center gap-3 text-xl tracking-[0.28em]"
        >
          <StarMark className="size-4" /> Starlights
        </a>
        <nav
          aria-label="Primary"
          className="hidden items-center gap-9 text-[0.75rem] font-medium tracking-[0.16em] text-ink/80 uppercase md:flex"
        >
          <a className="hover:text-gold" href="#builder">
            Builder
          </a>
          <a className="hover:text-gold" href="#also">
            Also in the works
          </a>
          <a className="hover:text-gold" href="#sky">
            Star map
          </a>
          <a
            className="hover:text-gold"
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
      className="on-dark relative isolate flex min-h-[100svh] items-end overflow-hidden"
    >
      <img
        src="/images/shar-hero.webp"
        alt="Selûne, a pale glowing woman, and Shar, a dark-skinned woman in spiked black armour, raise their hands together around a crescent moon eclipsed by a black disc in a violet night sky."
        className="settle absolute inset-0 -z-20 size-full object-cover object-[50%_28%]"
        fetchPriority="high"
        decoding="async"
      />
      <div className="absolute inset-0 -z-10 bg-linear-to-t from-[oklch(0.175_0.012_285)] via-[oklch(0.175_0.012_285/0.55)] via-35% to-[oklch(0.175_0.012_285/0.25)]" />
      <div className="absolute inset-x-0 top-0 -z-10 h-40 bg-linear-to-b from-[oklch(0.14_0.012_285/0.7)] to-transparent" />

      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-0 -z-10 overflow-hidden"
      >
        {dust.map((p, i) => (
          <span
            key={i}
            className="dust"
            style={
              {
                left: `${p.left}%`,
                width: p.size,
                height: p.size,
                "--t": `${p.t}s`,
                "--d": `${p.d}s`,
                "--dx": `${p.dx}px`,
                "--o": p.o,
                "--c": p.c,
              } as CSSProperties
            }
          />
        ))}
      </div>

      <div className={`${container} pt-40 pb-20 text-center sm:pb-28`}>
        <p className="eyebrow rise text-gold">
          Project Starlights &middot; early development
        </p>
        <h1
          className="display rise mt-7 text-[clamp(3.5rem,11vw,9rem)] leading-[0.92] tracking-tight"
          style={{ "--d": "100ms" } as CSSProperties}
        >
          Heroes <span className="text-gold">first.</span>
        </h1>
        <p
          className="rise display mt-6 text-[clamp(1rem,2.3vw,1.6rem)] font-normal tracking-[0.16em] text-mute"
          style={{ "--d": "220ms" } as CSSProperties}
        >
          The rest of the table follows.
        </p>
      </div>
    </section>
  )
}

function Builder() {
  return (
    <section id="builder">
      <div className={`${container} py-24 sm:py-32`}>
        <div className="grid items-start gap-16 lg:grid-cols-12 lg:gap-20">
          <Reveal className="lg:col-span-5">
            <p className="eyebrow text-gold">
              The builder &middot; {builderTile.tag}
            </p>
            <h2 className="display mt-6 text-[clamp(2.5rem,5vw,4rem)] leading-[0.98]">
              Build the hero.
              <span className="block text-gold">Keep the reasoning.</span>
            </h2>
            <p className="mt-7 max-w-md leading-relaxed text-mute">
              Most character builders hand you a finished number. Starlights
              keeps the working attached, so you always know where a modifier
              came from and what a choice will cost you later.
            </p>

            <ul className="mt-10 divide-y divide-rule border-y border-rule">
              {points.map((point) => (
                <li key={point.title} className="py-4">
                  <h3 className="display text-lg font-medium tracking-wider">
                    {point.title}
                  </h3>
                  <p className="mt-1.5 text-sm leading-relaxed text-mute">
                    {point.body}
                  </p>
                </li>
              ))}
            </ul>

            <div className="mt-9 flex flex-wrap items-center gap-x-8 gap-y-4">
              <a
                href={builderHref}
                className="btn-solid inline-flex items-center gap-2 rounded-sm px-6 py-3.5"
              >
                {builderCta}{" "}
                <ArrowRight className="size-4" aria-hidden="true" />
              </a>
              <span className="text-sm text-mute">
                {statusLabel[builderTile.status]} &middot; first-release
                targets, not shipped features
              </span>
            </div>
          </Reveal>

          <Reveal className="lg:col-span-7" delay={150}>
            <div className="relative">
              <img
                src="/images/builder-dragon.webp"
                alt="A green dragon with antler-like horns spreads its wings on a mossy stone arch above a pine forest and a ruined tower."
                loading="lazy"
                decoding="async"
                className="aspect-[4/3] w-full rounded-sm object-cover object-[48%_30%]"
              />
              <figure className="mt-4 sm:absolute sm:-bottom-12 sm:-left-12 sm:mt-0 sm:w-64">
                <img
                  src="/images/scene-dice.webp"
                  alt="A painted ranger miniature stands among indigo and gold polyhedral dice on a dark table."
                  loading="lazy"
                  decoding="async"
                  className="aspect-[3/2] w-full rounded-sm border-4 border-sky object-cover shadow-[0_24px_40px_-20px_rgb(0_0_0/0.9)]"
                />
              </figure>
            </div>
          </Reveal>
        </div>
      </div>
    </section>
  )
}

const also = [
  {
    tile: campaignTile,
    image: "/images/builder-drow.webp",
    alt: "A white-haired dark elf sorceress sits on a stone throne with a glowing violet staff, flanked by banners, grey statues and two giant spiders.",
    position: "50% 40%",
  },
  {
    tile: compendiumTile,
    image: "/images/builder-compendium.webp",
    alt: "A robed scholar stands in a vast round library as spectral blue wisps lift glowing books into the air.",
    position: "55% 40%",
  },
]

function Also() {
  return (
    <section id="also" className="border-t border-rule">
      <div className={`${container} pt-24 sm:pt-32`}>
        <Reveal>
          <p className="eyebrow text-gold">Also in the works</p>
          <h2 className="display mt-6 text-[clamp(2.25rem,4.5vw,3.5rem)]">
            Planned, in their own time
          </h2>
        </Reveal>
      </div>

      {also.map(({ tile, image, alt, position }, i) => (
        <div key={tile.id} className={i > 0 ? "border-t border-rule" : ""}>
          <div
            className={`${container} grid items-center gap-10 py-16 sm:py-24 lg:grid-cols-12 lg:gap-20`}
          >
            <Reveal className={`lg:col-span-7 ${i % 2 ? "lg:order-2" : ""}`}>
              <img
                src={image}
                alt={alt}
                loading="lazy"
                decoding="async"
                className="aspect-[16/9] w-full rounded-sm object-cover"
                style={{ objectPosition: position }}
              />
            </Reveal>
            <Reveal
              className={`lg:col-span-5 ${i % 2 ? "lg:order-1" : ""}`}
              delay={120}
            >
              <p className="eyebrow text-mute">{statusLabel[tile.status]}</p>
              <h3 className="display mt-5 text-4xl sm:text-5xl">
                {tile.title}
              </h3>
              <p className="mt-6 max-w-md leading-relaxed text-mute">
                {tile.blurb}
              </p>
            </Reveal>
          </div>
        </div>
      ))}
    </section>
  )
}

function SkySection() {
  return (
    <section id="sky" className="border-t border-rule">
      <div className={`${container} py-24 sm:py-32`}>
        <Reveal className="mx-auto max-w-3xl text-center">
          <p className="eyebrow text-gold">The star map</p>
          <h2 className="display mt-6 text-[clamp(2.5rem,5.5vw,4.5rem)] leading-[0.98]">
            One builder today.
            <span className="block text-gold">A whole sky later.</span>
          </h2>
          <p className="mx-auto mt-7 max-w-xl leading-relaxed text-mute">
            Starlights is built as separate modules on a shared platform, so new
            tools and new games can arrive one at a time. Nothing past the
            builder is promised, only intended.
          </p>
        </Reveal>

        <div className="mt-16">
          <Sky
            layout={armillaryLayout}
            size={[1000, 560]}
            decor={<Armillary />}
            stars={140}
            seed={33}
            side
            bend={0.06}
            frameClass="rounded-sm border border-rule bg-deep"
            chartClass="bg-[radial-gradient(ellipse_at_50%_50%,var(--v4-nebula),transparent_70%)]"
            nameClass="display text-[0.8rem] font-medium tracking-[0.1em] sm:text-sm"
            titleClass="display tracking-[0.06em]"
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
    <section className="border-t border-rule bg-deep">
      <Reveal className={`${container} py-24 text-center sm:py-32`}>
        <StarMark className="mx-auto size-5" />
        <h2 className="display mx-auto mt-7 max-w-3xl text-[clamp(2.25rem,4.5vw,3.5rem)] leading-[1.02]">
          Open source, growing{" "}
          <span className="text-gold">slowly, in public.</span>
        </h2>
        <p className="mx-auto mt-6 max-w-lg leading-relaxed text-mute">
          Project Starlights is MIT-licensed and picks up one small piece at a
          time. A star on GitHub is the kindest way to say it is worth
          continuing.
        </p>
        <a
          href={REPO_URL}
          target="_blank"
          rel="noreferrer"
          className="btn-line mt-10 inline-flex items-center gap-2 rounded-sm px-6 py-3.5"
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
    <footer className="border-t border-rule">
      <div
        className={`${container} grid gap-8 py-12 text-sm text-mute md:grid-cols-12`}
      >
        <div className="md:col-span-4">
          <p className="display flex items-center gap-3 text-lg tracking-[0.28em] text-ink">
            <StarMark className="size-4" /> Starlights
          </p>
          <VariantLinks
            current="v4"
            className="mt-5"
            linkClass="hover:text-ink"
            activeClass="text-gold underline underline-offset-4"
          />
        </div>
        <p className="md:col-span-4">
          {artNote} The eclipse artwork is a personal reference image, used here
          only for local prototyping.
        </p>
        <p className="md:col-span-4">{trademarkNote}</p>
      </div>
    </footer>
  )
}

export default function V4() {
  return (
    <div className="v4">
      <Header />
      <main>
        <Hero />
        <Builder />
        <Also />
        <SkySection />
        <Follow />
      </main>
      <Footer />
    </div>
  )
}
