import { ArrowUpRight } from "lucide-react"

import { REPO_URL } from "../data/content"
import { Reveal } from "./reveal"
import { containerClass } from "./site-header"

const points = [
  {
    title: "Every SRD class, levels 1 to 20",
    body: "Including multiclassing, and starting a new character above level 1.",
  },
  {
    title: "Choices that explain themselves",
    body: "Each decision says what it changes on the sheet, so you can revisit it without breaking anything.",
  },
  {
    title: "Gear that counts",
    body: "Equipment, magic items and attunement feed straight into armor class, attacks and carrying.",
  },
  {
    title: "A sheet for the table",
    body: "Readable on screen, exportable to PDF, in light or dark.",
  },
]

export function BuilderSection() {
  return (
    <section id="builder" className="relative isolate border-t border-rule">
      <div className={`${containerClass} py-20 sm:py-32`}>
        <div className="grid items-center gap-14 lg:grid-cols-12 lg:gap-20">
          <Reveal className="lg:col-span-7">
            <p className="eyebrow text-gold">The builder</p>
            <h2 className="display mt-5 text-5xl leading-[0.95] sm:text-6xl">
              A sheet that <span className="text-gold">shows its work.</span>
            </h2>
            <p className="mt-6 leading-relaxed text-mute">
              Most character builders hand you a finished number. Starlights
              keeps the reasoning attached, so you always know where a modifier
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

            <p className="mt-6 text-sm text-mute">
              These are first-release targets, not shipped features.{" "}
              <a
                className="inline-flex items-center gap-0.5 text-ink underline decoration-gold underline-offset-4"
                href={`${REPO_URL}/blob/main/docs/ROADMAP.md`}
                target="_blank"
                rel="noreferrer"
              >
                Follow the roadmap
                <ArrowUpRight className="size-3.5" aria-hidden="true" />
              </a>
            </p>
          </Reveal>

          <Reveal className="lg:col-span-5" delay={150}>
            <figure className="mx-auto max-w-md">
              <div className="overflow-hidden rounded-xl border-4 border-double border-ink/30 shadow-[0_40px_80px_-40px_rgb(0_0_0/0.7)]">
                <img
                  src="/images/sheet.webp"
                  alt="A filled-in ranger character sheet on a dark wooden table, beside candlelight, polyhedral dice, a pencil and a painted ranger miniature."
                  loading="lazy"
                  decoding="async"
                  className="aspect-[2/3] w-full object-cover saturate-[0.9]"
                />
              </div>
              <figcaption className="mt-3 text-xs text-mute">
                Prototype art: an example of the sheet we&rsquo;re building
                toward, not a screenshot.
              </figcaption>
            </figure>
          </Reveal>
        </div>
      </div>
    </section>
  )
}
