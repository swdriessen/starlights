import { Button } from "ui-framework/button"
import { Card, CardContent, CardFooter, CardHeader } from "ui-framework/card"

export function App() {
  return (
    <div className="flex min-h-svh p-6">
      <div className="flex max-w-md min-w-0 flex-col gap-4 text-sm leading-loose">
        <div>
          <h1 className="font-medium">Component Library Showcase</h1>
          <div className="flex flex-col gap-3">
            <Card>
              <CardHeader>
                <h2 className="font-medium">Card Title</h2>
              </CardHeader>
              <CardContent>
                <p>Card content goes here.</p>
              </CardContent>
              <CardFooter>
                <Button>Action Button</Button>
              </CardFooter>
            </Card>
          </div>
        </div>
        <div className="font-mono text-xs text-muted-foreground">
          (Press <kbd>d</kbd> to toggle dark mode)
        </div>
      </div>
    </div>
  )
}

export default App
