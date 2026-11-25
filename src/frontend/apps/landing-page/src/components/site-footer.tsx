import { ArrowUpRight } from "lucide-react"
import { Button } from "ui-framework"

import { REPO_URL } from "../data/content"
import { VariantLinks } from "../variants/variant-links"
import { StarMark } from "./glyphs"
import { Reveal } from "./reveal"
import { containerClass } from "./site-header"

export function FollowSection() {
  return (
    <section className="border-t border-rule">
      <div className={`${containerClass} py-20 sm:py-28`}>
        <Reveal>
          <div className="relative mt-14 rounded-2xl border border-rule bg-plate md:mt-20">
            <div className="pointer-events-none relative h-44 [clip-path:inset(-60%_0_0_0_round_0_0_1rem_1rem)] md:absolute md:inset-0 md:h-auto">
              <img
                src="/images/wizard-cutout.webp"
                alt="A silver-haired half-elf wizard in a dark star-embroidered hooded cloak holds a small glowing four-pointed star."
                loading="lazy"
                decoding="async"
                className="absolute right-0 bottom-0 h-[135%] w-auto max-w-none saturate-[0.9]"
              />
            </div>

            <div className="relative p-8 sm:p-10 md:w-[62%] lg:p-12">
              <p className="eyebrow text-gold">Follow along</p>
              <h2 className="display mt-4 text-3xl leading-[0.98] sm:text-5xl">
                Open source, growing{" "}
                <span className="text-gold">slowly, in public.</span>
              </h2>
              <p className="mt-5 max-w-lg leading-relaxed text-mute">
                Project Starlights is MIT-licensed and picks up one small piece
                at a time. A star on GitHub is the kindest way to say it&rsquo;s
                worth continuing.
              </p>
              <div className="mt-6">
                <Button
                  size="lg"
                  className="btn-accent h-11 rounded-md border-0 px-5 text-[0.8125rem] font-semibold tracking-wide"
                  nativeButton={false}
                  render={
                    <a href={REPO_URL} target="_blank" rel="noreferrer" />
                  }
                >
                  Star it on GitHub
                  <ArrowUpRight data-icon="inline-end" aria-hidden="true" />
                </Button>
              </div>
            </div>
          </div>
        </Reveal>
      </div>
    </section>
  )
}

export function SiteFooter() {
  return (
    <footer className="border-t border-rule">
      <div
        className={`${containerClass} grid gap-10 py-12 text-sm text-mute md:grid-cols-12`}
      >
        <div className="md:col-span-4">
          <p className="flex items-center gap-3 text-ink">
            <StarMark className="size-4" />
            <span className="display text-lg tracking-[0.2em]">Starlights</span>
          </p>
          <p className="mt-3 leading-relaxed">
            Press{" "}
            <kbd className="rounded-sm border border-rule px-1.5 py-0.5 font-mono text-xs text-ink">
              D
            </kbd>{" "}
            to switch between light and dark.
          </p>
          <VariantLinks
            current="v0"
            className="mt-5 text-sm"
            linkClass="transition-colors hover:text-ink"
            activeClass="text-gold underline underline-offset-4"
          />
        </div>

        <p className="leading-relaxed md:col-span-4">
          Prototype art: the ranger, campaign table and character sheet are
          AI-generated for this mock-up, and the compendium scene is borrowed
          from the builder prototype. Nothing here is published or licensed for
          release.
        </p>

        <p className="leading-relaxed md:col-span-4">
          Dungeons &amp; Dragons, Daggerheart and Pathfinder are trademarks of
          their respective owners. Project Starlights is an independent fan
          project, not affiliated with or endorsed by them, and mentioning a
          game here is an idea, not an announcement.
        </p>
      </div>
    </footer>
  )
}
