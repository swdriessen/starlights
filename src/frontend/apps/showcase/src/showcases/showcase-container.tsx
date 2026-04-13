import type { ReactNode } from "react"

type ShowcaseContainerProps = {
  title: string
  description?: string
  children: ReactNode
}

export function ShowcaseContainer({
  title,
  description,
  children,
}: ShowcaseContainerProps) {
  return (
    <section className="flex flex-col gap-4">
      <header className="flex flex-col gap-1">
        <h2 className="text-base font-medium tracking-tight">{title}</h2>
        {description ? (
          <p className="text-xs text-muted-foreground">{description}</p>
        ) : null}
      </header>

      <div className="rounded-2xl border border-dashed border-border/80 bg-transparent p-6">
        {children}
      </div>
    </section>
  )
}
