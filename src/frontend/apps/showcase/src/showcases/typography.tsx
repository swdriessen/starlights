import { ShowcaseContainer } from "./showcase-container"

export function TypographyShowcase() {
  return (
    <ShowcaseContainer
      title="Typography"
      description="Baseline text hierarchy without dedicated typography CSS yet."
    >
      <div className="flex flex-col gap-6">
        <section className="flex flex-col gap-2">
          <h1 className="text-3xl font-semibold tracking-tight text-foreground">
            Heading 1
          </h1>
          <h2 className="text-2xl font-semibold tracking-tight text-foreground">
            Heading 2
          </h2>
          <h3 className="text-xl font-semibold tracking-tight text-foreground">
            Heading 3
          </h3>
          <h4 className="text-lg font-medium tracking-tight text-foreground">
            Heading 4
          </h4>
        </section>

        <section className="flex flex-col gap-3">
          <p className="text-sm text-foreground">
            This is a default paragraph style for regular content blocks in the
            showcase app.
          </p>
          <p className="text-sm text-muted-foreground">
            Muted copy can be used for supporting content, helper text, or
            context that should not dominate the page.
          </p>
        </section>

        <section className="flex flex-col gap-3">
          <blockquote className="border-l-2 border-border pl-3 text-sm text-muted-foreground italic">
            Good typography improves scanning speed and makes component demos
            easier to understand.
          </blockquote>
          <ul className="list-disc space-y-1 pl-5 text-sm text-foreground">
            <li>Readable hierarchy for headings and body text</li>
            <li>Consistent spacing between related text blocks</li>
            <li>Muted style for secondary information</li>
          </ul>
          <code className="w-fit rounded-md border border-border/80 bg-muted px-2 py-1 font-mono text-xs">
            npm run showcase
          </code>
        </section>
      </div>
    </ShowcaseContainer>
  )
}
