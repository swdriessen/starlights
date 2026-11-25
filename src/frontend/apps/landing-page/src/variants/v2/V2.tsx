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
import { artNote, points, trademarkNote } from "../copy"
import { Sky, type Layout } from "../sky"
import { VariantLinks } from "../variant-links"
import "./v2.css"

const container = "mx-auto w-full max-w-[1180px] px-5 sm:px-8"

/** A winding trail from the builder (left) toward the horizon, with goals spread along it. */
const layout: Layout = {
  builder: { x: 12, y: 54, label: "bottom" },
  compendium: { x: 24, y: 22, label: "top" },
  homebrew: { x: 33, y: 68, label: "bottom" },
  campaigns: { x: 46, y: 18, label: "top" },
  dnd5e: { x: 56, y: 50, label: "bottom" },
  daggerheart: { x: 74, y: 32, label: "bottom" },
  pathfinder: { x: 80, y: 66, label: "bottom" },
  dnd4e: { x: 50, y: 78, label: "bottom" },
  more: { x: 92, y: 12, label: "bottom" },
}

function Road() {
  return (
    <g transform="translate(0 90)">
      <defs>
        <linearGradient id="v2-glow" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0" stopColor="oklch(0.6 0.15 50)" stopOpacity="0" />
          <stop offset="1" stopColor="oklch(0.6 0.15 50)" stopOpacity="0.22" />
        </linearGradient>
      </defs>
      <rect x="0" y="180" width="1000" height="260" fill="url(#v2-glow)" />
      <path
        d="M0 400 C120 350 200 372 300 392 C420 416 520 340 640 366 C760 392 880 340 1000 372 L1000 440 L0 440 Z"
        fill="var(--l-deep)"
        opacity="0.85"
      />
      <path
        d="M40 392 C150 340 230 380 340 346 C450 312 520 380 640 330 C760 280 840 330 960 250"
        fill="none"
        stroke="var(--l-gold)"
        strokeOpacity="0.35"
        strokeWidth="2"
        strokeDasharray="1 11"
        strokeLinecap="round"
      />
    </g>
  )
}

const embers = Array.from({ length: 26 }, (_, i) => ({
  left: (i * 37 + 9) % 100,
  size: 2 + (i % 3),
  t: 7 + (i % 5) * 1.6,
  d: -((i * 0.83) % 9),
  dx: ((i % 7) - 3) * 16,
  o: 0.5 + (i % 4) * 0.15,
}))

function Header() {
  return (
    <header className="absolute inset-x-0 top-0 z-20">
      <div className={`${container} flex h-20 items-center justify-between`}>
        <a href="#top" className="display flex items-center gap-3 text-2xl">
          <StarMark className="size-5" /> Starlights
        </a>
        <nav
          aria-label="Primary"
          className="hidden items-center gap-8 text-base font-semibold text-ink/80 sm:flex"
        >
          <a className="hover:text-gold" href="#notices">
            Notices
          </a>
          <a className="hover:text-gold" href="#builder">
            The builder
          </a>
          <a className="hover:text-gold" href="#sky">
            The road ahead
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
      className="relative isolate flex min-h-[92svh] items-end overflow-hidden"
    >
      <img
        src="/images/campfire.webp"
        alt="Four adventurers, a dwarf, an elf, a halfling and a wizard, rest around a campfire in a dark forest clearing beside a ruin."
        className="settle absolute inset-0 -z-20 size-full object-cover object-[50%_65%]"
        fetchPriority="high"
        decoding="async"
      />
      <div className="absolute inset-0 -z-10 bg-linear-to-t from-[oklch(0.13_0.012_50)] via-[oklch(0.13_0.012_50/0.55)] to-[oklch(0.13_0.012_50/0.35)]" />
      <div className="absolute inset-0 -z-10 bg-linear-to-r from-[oklch(0.13_0.012_50/0.85)] via-transparent to-transparent" />

      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-0 -z-10 overflow-hidden"
      >
        {embers.map((e, i) => (
          <span
            key={i}
            className="ember"
            style={
              {
                left: `${e.left}%`,
                width: e.size,
                height: e.size,
                "--t": `${e.t}s`,
                "--d": `${e.d}s`,
                "--dx": `${e.dx}px`,
                "--o": e.o,
              } as CSSProperties
            }
          />
        ))}
      </div>

      <div className={`${container} pt-40 pb-16 sm:pb-24`}>
        <p className="eyebrow rise text-gold">
          Early development &middot; SRD 5.2.1
        </p>
        <h1
          className="display rise mt-5 text-6xl sm:text-8xl"
          style={{ "--d": "100ms" } as CSSProperties}
        >
          Pull up a chair.
        </h1>
        <p
          className="rise mt-6 max-w-xl text-xl leading-relaxed text-ink/85"
          style={{ "--d": "220ms" } as CSSProperties}
        >
          Starlights is a character builder for tabletop roleplaying games.
          Start with a hero by the fire; the campaigns, the compendium and the
          rest of the party will find their seats in time.
        </p>
        <div
          className="rise mt-9 flex flex-wrap items-center gap-x-8 gap-y-4"
          style={{ "--d": "340ms" } as CSSProperties}
        >
          <a
            href={builderHref}
            className="btn-ember inline-flex items-center gap-2 rounded-md px-6 py-3 text-base"
          >
            {builderCta} <ArrowRight className="size-4" aria-hidden="true" />
          </a>
          <a
            href="#sky"
            className="text-base font-semibold text-ink/80 underline decoration-gold decoration-2 underline-offset-8 hover:text-ink"
          >
            See the road ahead
          </a>
        </div>
      </div>
    </section>
  )
}

const tilts = ["-1.4deg", "0.8deg", "-0.6deg"]

function Notices() {
  return (
    <section id="notices" className="relative">
      <div className={`${container} py-20 sm:py-28`}>
        <Reveal>
          <p className="eyebrow text-gold">On the notice board</p>
          <h2 className="display mt-4 text-4xl sm:text-5xl">
            What&rsquo;s on the table
          </h2>
        </Reveal>
        <div className="mt-14 grid gap-10 md:grid-cols-3">
          {tiles.map((tile, i) => (
            <Reveal key={tile.id} delay={i * 120}>
              <article
                className="notice relative rounded-lg border border-rule bg-plate p-3 pb-6 shadow-[0_24px_40px_-26px_rgb(0_0_0/0.9)]"
                style={{ transform: `rotate(${tilts[i]})` }}
              >
                <span
                  aria-hidden="true"
                  className="absolute -top-2 left-1/2 size-4 -translate-x-1/2 rounded-full bg-gold shadow-[0_2px_6px_rgb(0_0_0/0.6)]"
                />
                <img
                  src={tile.image}
                  alt={tile.alt}
                  loading="lazy"
                  decoding="async"
                  className="aspect-[4/3] w-full rounded-md object-cover saturate-[0.85]"
                  style={{ objectPosition: tile.position }}
                />
                <div className="px-3 pt-5">
                  <p className="eyebrow flex items-center gap-2 text-gold">
                    <span
                      className={`size-2 rounded-full ${tile.status === "building" ? "bg-gold" : "bg-ink/40"}`}
                    />
                    {statusLabel[tile.status]}
                    {tile.tag ? ` · ${tile.tag}` : ""}
                  </p>
                  <h3 className="display mt-3 text-2xl">{tile.title}</h3>
                  <p className="mt-3 leading-relaxed text-mute">{tile.blurb}</p>
                </div>
              </article>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

function Builder() {
  return (
    <section id="builder" className="bg-deep">
      <div
        className={`${container} grid items-center gap-16 py-20 sm:py-28 lg:grid-cols-12`}
      >
        <Reveal className="lg:col-span-6">
          <p className="eyebrow text-gold">The builder</p>
          <h2 className="display mt-4 text-4xl sm:text-5xl">
            A sheet that <span className="text-gold">shows its work.</span>
          </h2>
          <p className="mt-6 text-xl leading-relaxed text-mute">
            Most character builders hand you a finished number. Starlights keeps
            the reasoning attached, so you always know where a modifier came
            from and what a choice will cost you later.
          </p>
          <ul className="mt-9 space-y-6">
            {points.map((point) => (
              <li key={point.title} className="flex gap-4">
                <StarMark className="mt-1.5 size-3.5 shrink-0" />
                <div>
                  <h3 className="text-lg font-bold">{point.title}</h3>
                  <p className="mt-1 leading-relaxed text-mute">{point.body}</p>
                </div>
              </li>
            ))}
          </ul>
          <p className="mt-8 text-base text-mute italic">
            First-release targets, not shipped features.
          </p>
        </Reveal>

        <Reveal className="lg:col-span-6" delay={120}>
          <figure className="relative mx-auto max-w-md rotate-2">
            <span
              aria-hidden="true"
              className="absolute -top-3 left-6 z-10 h-6 w-20 -rotate-6 bg-[oklch(0.85_0.06_85/0.55)]"
            />
            <span
              aria-hidden="true"
              className="absolute -top-3 right-6 z-10 h-6 w-20 rotate-6 bg-[oklch(0.85_0.06_85/0.55)]"
            />
            <img
              src="/images/sheet.webp"
              alt="A filled-in ranger character sheet on a dark wooden table, with candlelight, dice, a pencil and a ranger miniature."
              loading="lazy"
              decoding="async"
              className="aspect-[2/3] w-full rounded-md object-cover shadow-[0_40px_70px_-30px_rgb(0_0_0/0.9)]"
            />
            <figcaption className="mt-3 text-center text-sm text-mute italic">
              A sheet such as the builder will keep.
            </figcaption>
          </figure>
        </Reveal>
      </div>
    </section>
  )
}

function SkySection() {
  return (
    <section id="sky">
      <div className={`${container} py-20 sm:py-28`}>
        <Reveal className="max-w-3xl">
          <p className="eyebrow text-gold">The road ahead</p>
          <h2 className="display mt-4 text-4xl sm:text-6xl">
            One builder today.{" "}
            <span className="text-gold">A whole sky later.</span>
          </h2>
          <p className="mt-6 text-xl leading-relaxed text-mute">
            Starlights is built as separate modules on a shared platform, so new
            tools and new games can arrive one at a time. Nothing past the
            builder is promised, only intended.
          </p>
        </Reveal>

        <div className="mt-12">
          <Sky
            layout={layout}
            size={[1000, 530]}
            decor={<Road />}
            stars={90}
            seed={5}
            bend={0.05}
            frameClass="rounded-xl border border-rule bg-deep"
            chartClass="bg-linear-to-b from-[oklch(0.12_0.012_50)] to-[oklch(0.2_0.04_50)]"
            nameClass="display text-base sm:text-lg"
            titleClass="display"
            lineStrong="var(--l-gold)"
            lineNear="var(--l-ink)"
            lineFar="var(--l-cool)"
            legendClass="rounded-md"
          />
        </div>
      </div>
    </section>
  )
}

function Follow() {
  return (
    <section className="relative overflow-hidden border-t border-rule bg-deep">
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-x-0 bottom-0 h-40 bg-linear-to-t from-[oklch(0.6_0.15_50/0.2)] to-transparent"
      />
      <Reveal className={`${container} relative py-20 text-center sm:py-24`}>
        <h2 className="display mx-auto max-w-3xl text-4xl sm:text-5xl">
          Leave a star <span className="text-gold">by the door.</span>
        </h2>
        <p className="mx-auto mt-5 max-w-xl text-xl leading-relaxed text-mute">
          Project Starlights is open source (MIT) and grows one small piece at a
          time. A star on GitHub is the kindest way to say it is worth
          continuing.
        </p>
        <a
          href={REPO_URL}
          target="_blank"
          rel="noreferrer"
          className="btn-ember mt-8 inline-flex items-center gap-2 rounded-md px-6 py-3 text-base"
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
        className={`${container} grid gap-8 py-10 text-base text-mute md:grid-cols-12`}
      >
        <div className="md:col-span-4">
          <p className="display flex items-center gap-3 text-xl text-ink">
            <StarMark className="size-4" /> Starlights
          </p>
          <VariantLinks
            current="v2"
            className="mt-4"
            linkClass="hover:text-ink"
            activeClass="text-gold underline underline-offset-4"
          />
        </div>
        <p className="text-sm md:col-span-4">{artNote}</p>
        <p className="text-sm md:col-span-4">{trademarkNote}</p>
      </div>
    </footer>
  )
}

export default function V2() {
  return (
    <div className="v2">
      <Header />
      <main>
        <Hero />
        <Notices />
        <Builder />
        <SkySection />
        <Follow />
      </main>
      <Footer />
    </div>
  )
}
