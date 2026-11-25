import { ArrowUpRight } from "lucide-react"
import type { CSSProperties } from "react"

import { StarMark } from "../../components/glyphs"
import { Reveal } from "../../components/reveal"
import { Starfield } from "../../components/starfield"
import {
  builderCta,
  builderHref,
  REPO_URL,
  statusLabel,
  tiles,
} from "../../data/content"
import { artNote, points, trademarkNote } from "../copy"
import { Armillary } from "../armillary"
import { armillaryLayout as layout } from "../armillary-layout"
import { Sky } from "../sky"
import { VariantLinks } from "../variant-links"
import "./v3.css"

const container = "mx-auto w-full max-w-[1180px] px-5 sm:px-8"

function Header() {
  return (
    <header className="sticky top-0 z-30 border-b border-rule bg-sky/80 backdrop-blur-md">
      <div className={`${container} flex h-16 items-center justify-between`}>
        <a
          href="#top"
          className="display flex items-center gap-3 text-lg tracking-[0.3em]"
        >
          <StarMark className="size-4" /> Starlights
        </a>
        <nav
          aria-label="Primary"
          className="eyebrow hidden items-center gap-9 text-mute sm:flex"
        >
          <a className="hover:text-ink" href="#realms">
            Realms
          </a>
          <a className="hover:text-ink" href="#builder">
            Builder
          </a>
          <a className="hover:text-ink" href="#sky">
            Armillary
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
    <section id="top" className="relative isolate overflow-hidden">
      <Starfield
        count={70}
        seed={11}
        className="[mask-image:linear-gradient(to_bottom,black,transparent_85%)]"
      />
      <div
        className={`${container} grid items-center gap-12 pt-14 pb-20 lg:grid-cols-12 lg:pt-20 lg:pb-28`}
      >
        <div className="lg:col-span-7">
          <p className="eyebrow rise text-gold">
            Early development &middot; SRD 5.2.1
          </p>
          <h1
            className="display rise mt-7 text-5xl sm:text-7xl"
            style={{ "--d": "100ms" } as CSSProperties}
          >
            Heroes
            <br />
            <span className="text-gold">first.</span>
          </h1>
          <div
            className="leaf-rule rise mt-8 w-40"
            style={{ "--d": "200ms" } as CSSProperties}
          />
          <p
            className="rise mt-8 max-w-lg leading-relaxed text-mute italic"
            style={{ "--d": "260ms" } as CSSProperties}
          >
            A character builder for tabletop roleplaying games, kept quiet and
            exact. It is the first light in a larger sky: campaigns, a
            compendium, homebrew tools and other rulesets, each in its own time.
          </p>
          <div
            className="rise mt-10 flex flex-wrap items-center gap-x-9 gap-y-4"
            style={{ "--d": "360ms" } as CSSProperties}
          >
            <a href={builderHref} className="btn-moon eyebrow px-7 py-4">
              {builderCta}
            </a>
            <a
              href="#sky"
              className="eyebrow border-b border-gold/60 pb-1 text-mute hover:text-ink"
            >
              Open the armillary
            </a>
          </div>
        </div>

        <div
          className="rise lg:col-span-5"
          style={{ "--d": "200ms" } as CSSProperties}
        >
          <div className="arch relative mx-auto aspect-[2/3] max-w-sm overflow-hidden border border-gold/50 p-2">
            <div className="arch size-full overflow-hidden">
              <img
                src="/images/observatory.webp"
                alt="A moonlit elven observatory: a vine-wrapped stone arch frames a great brass armillary sphere above a forested valley and a crescent moon."
                className="settle size-full object-cover"
                fetchPriority="high"
                decoding="async"
              />
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

function Realms() {
  return (
    <section id="realms" className="border-t border-rule">
      <div className={`${container} py-20 sm:py-28`}>
        <Reveal className="text-center">
          <p className="eyebrow text-gold">Three realms</p>
          <h2 className="display mt-5 text-3xl sm:text-4xl">
            The first of many
          </h2>
          <div className="leaf-rule mx-auto mt-6 w-24" />
        </Reveal>
        <div className="mt-14 grid gap-px overflow-hidden rounded-sm border border-rule bg-rule md:grid-cols-3">
          {tiles.map((tile, i) => (
            <Reveal key={tile.id} delay={i * 120} className="bg-sky">
              <article className="group relative h-full">
                <div className="relative aspect-[4/5] overflow-hidden">
                  <img
                    src={tile.image}
                    alt={tile.alt}
                    loading="lazy"
                    decoding="async"
                    className="size-full object-cover saturate-[0.75] transition duration-1000 group-hover:scale-[1.04] group-hover:saturate-100"
                    style={{ objectPosition: tile.position }}
                  />
                  <div className="absolute inset-0 bg-linear-to-t from-[oklch(0.16_0.014_165)] via-[oklch(0.16_0.014_165/0.2)] to-transparent" />
                </div>
                <div className="relative -mt-24 px-7 pb-9">
                  <p className="eyebrow text-gold">
                    {statusLabel[tile.status]}
                    {tile.tag ? ` · ${tile.tag}` : ""}
                  </p>
                  <h3 className="display mt-4 text-2xl">{tile.title}</h3>
                  <p className="mt-4 leading-relaxed text-mute italic">
                    {tile.blurb}
                  </p>
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
    <section id="builder" className="border-t border-rule bg-deep">
      <div
        className={`${container} grid items-center gap-16 py-20 sm:py-28 lg:grid-cols-12`}
      >
        <Reveal className="order-2 lg:order-1 lg:col-span-7">
          <p className="eyebrow text-gold">The builder</p>
          <h2 className="display mt-5 text-3xl sm:text-5xl">
            A sheet that <span className="text-gold">shows its work.</span>
          </h2>
          <p className="mt-7 max-w-xl leading-relaxed text-mute italic">
            Most character builders hand you a finished number. Starlights keeps
            the reasoning attached, so you always know where a modifier came
            from and what a choice will cost you later.
          </p>
          <dl className="mt-10 grid gap-x-10 gap-y-8 sm:grid-cols-2">
            {points.map((point) => (
              <div key={point.title} className="border-t border-gold/40 pt-4">
                <dt className="display text-base tracking-[0.1em]">
                  {point.title}
                </dt>
                <dd className="mt-2 text-lg leading-relaxed text-mute">
                  {point.body}
                </dd>
              </div>
            ))}
          </dl>
          <p className="mt-9 text-mute italic">
            First-release targets, not shipped features.
          </p>
        </Reveal>

        <Reveal className="order-1 lg:order-2 lg:col-span-5" delay={120}>
          <figure className="mx-auto max-w-sm">
            <div className="arch overflow-hidden border border-gold/50 p-2">
              <img
                src="/images/sheet.webp"
                alt="A filled-in ranger character sheet on a dark wooden table, with candlelight, dice, a pencil and a ranger miniature."
                loading="lazy"
                decoding="async"
                className="arch aspect-[2/3] w-full object-cover saturate-[0.85]"
              />
            </div>
            <figcaption className="mt-4 text-center text-mute italic">
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
    <section id="sky" className="border-t border-rule">
      <div className={`${container} py-20 sm:py-28`}>
        <Reveal className="mx-auto max-w-2xl text-center">
          <p className="eyebrow text-gold">The armillary</p>
          <h2 className="display mt-5 text-3xl sm:text-5xl">
            One builder today.{" "}
            <span className="text-gold">A whole sky later.</span>
          </h2>
          <p className="mt-7 leading-relaxed text-mute italic">
            Starlights is built as separate modules on a shared platform, so new
            tools and new games can arrive one at a time. Nothing past the
            builder is promised, only intended.
          </p>
        </Reveal>

        <div className="mt-14">
          <Sky
            layout={layout}
            size={[1000, 560]}
            decor={<Armillary />}
            stars={140}
            seed={33}
            side
            bend={0.06}
            frameClass="rounded-sm border border-gold/30 bg-deep"
            chartClass="bg-[radial-gradient(ellipse_at_50%_50%,oklch(0.24_0.03_170/0.55),transparent_70%)]"
            nameClass="display text-[0.8rem] tracking-[0.12em] sm:text-sm"
            kindClass="tracking-[0.3em]"
            titleClass="display tracking-[0.08em]"
            lineStrong="var(--l-gold)"
            lineNear="var(--v3-silver)"
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
      <Reveal className={`${container} py-20 text-center sm:py-28`}>
        <StarMark className="mx-auto size-6" />
        <h2 className="display mx-auto mt-7 max-w-3xl text-3xl sm:text-4xl">
          Grown slowly, <span className="text-gold">in the open.</span>
        </h2>
        <p className="mx-auto mt-6 max-w-xl leading-relaxed text-mute italic">
          Project Starlights is MIT-licensed. A star on GitHub is the kindest
          way to say it is worth continuing.
        </p>
        <a
          href={REPO_URL}
          target="_blank"
          rel="noreferrer"
          className="btn-moon eyebrow mt-10 inline-flex items-center gap-3 px-7 py-4"
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
          <p className="display flex items-center gap-3 text-base tracking-[0.3em] text-ink">
            <StarMark className="size-4" /> Starlights
          </p>
          <VariantLinks
            current="v3"
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

export default function V3() {
  return (
    <div className="v3">
      <Header />
      <main>
        <Hero />
        <Realms />
        <Builder />
        <SkySection />
        <Follow />
      </main>
      <Footer />
    </div>
  )
}
