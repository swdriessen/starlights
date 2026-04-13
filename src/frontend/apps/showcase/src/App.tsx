import { ButtonShowcase } from "./showcases/button"
import { CheckboxShowcase } from "./showcases/checkbox"
import { IconButtonShowcase } from "./showcases/icon-button"
import { TypographyShowcase } from "./showcases/typography"

const showcaseSections = [
  {
    id: "button",
    label: "Button",
    render: () => <ButtonShowcase />,
  },
  {
    id: "checkbox",
    label: "Checkbox",
    render: () => <CheckboxShowcase />,
  },
  {
    id: "icon-button",
    label: "Icon Button",
    render: () => <IconButtonShowcase />,
  },
  {
    id: "typography",
    label: "Typography",
    render: () => <TypographyShowcase />,
  },
]

export function App() {
  return (
    <div className="flex min-h-svh justify-center p-6 md:p-10">
      <div className="flex w-full max-w-6xl min-w-0 gap-6 text-sm leading-loose lg:gap-8">
        <aside className="sticky top-6 hidden h-fit w-56 shrink-0 lg:block">
          <div className="rounded-xl border border-dashed border-border/80 bg-transparent p-3">
            <p className="text-xs font-semibold tracking-wide text-muted-foreground uppercase">
              Showcases
            </p>
            <nav className="mt-3 flex flex-col gap-1">
              {showcaseSections.map((section) => (
                <a
                  key={section.id}
                  href={`#${section.id}`}
                  className="rounded-md px-2 py-1.5 text-sm text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
                >
                  {section.label}
                </a>
              ))}
            </nav>
          </div>
        </aside>

        <main className="flex min-w-0 flex-1 flex-col gap-6">
          <div>
            <h1 className="font-medium">Component Library Showcase</h1>
            <nav className="mt-3 flex gap-2 overflow-x-auto pb-1 lg:hidden">
              {showcaseSections.map((section) => (
                <a
                  key={section.id}
                  href={`#${section.id}`}
                  className="rounded-md border border-border/80 px-2 py-1 text-xs whitespace-nowrap text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
                >
                  {section.label}
                </a>
              ))}
            </nav>

            <div className="mt-4 flex flex-col gap-6">
              {showcaseSections.map((section) => (
                <div key={section.id} id={section.id} className="scroll-mt-24">
                  {section.render()}
                </div>
              ))}
            </div>
          </div>
          <div className="font-mono text-xs text-muted-foreground">
            (Press <kbd>d</kbd> to toggle dark mode)
          </div>
        </main>
      </div>
    </div>
  )
}

export default App
