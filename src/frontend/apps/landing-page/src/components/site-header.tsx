import { ArrowUpRight, Moon, Sun } from "lucide-react"
import { Button, useTheme } from "ui-framework"

import { builderCta, builderHref, REPO_URL } from "../data/content"
import { StarMark } from "./glyphs"

export const containerClass = "mx-auto w-full max-w-[1200px] px-5 sm:px-8"

export function SiteHeader() {
  const { setTheme } = useTheme()

  const toggleTheme = () => {
    const isDark = document.documentElement.classList.contains("dark")
    setTheme(isDark ? "light" : "dark")
  }

  return (
    <header className="sticky top-0 z-30 border-b border-rule bg-sky/75 backdrop-blur-md">
      <div
        className={`${containerClass} flex h-16 items-center justify-between gap-6`}
      >
        <a href="#top" className="flex items-center gap-3">
          <StarMark className="size-5" />
          <span className="display text-xl tracking-[0.2em]">Starlights</span>
        </a>

        <nav
          aria-label="Primary"
          className="hidden items-center gap-8 text-[0.8125rem] font-medium tracking-wider text-mute uppercase sm:flex"
        >
          <a className="transition-colors hover:text-ink" href="#builder">
            Builder
          </a>
          <a className="transition-colors hover:text-ink" href="#sky">
            Star map
          </a>
          <a
            className="inline-flex items-center gap-1 transition-colors hover:text-ink"
            href={REPO_URL}
            target="_blank"
            rel="noreferrer"
          >
            GitHub <ArrowUpRight className="size-3.5" aria-hidden="true" />
          </a>
        </nav>

        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon-lg"
            onClick={toggleTheme}
            aria-label="Toggle light and dark theme"
          >
            <Sun className="hidden size-4 dark:block" aria-hidden="true" />
            <Moon className="size-4 dark:hidden" aria-hidden="true" />
          </Button>
          <Button
            size="lg"
            className="btn-accent h-10 rounded-md border-0 px-5 text-[0.8125rem] font-semibold tracking-wide"
            nativeButton={false}
            render={<a href={builderHref} />}
          >
            {builderCta}
          </Button>
        </div>
      </div>
    </header>
  )
}
