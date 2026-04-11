import { Button } from "ui-framework/button"
import { Card, CardContent } from "ui-framework/card"

export function App() {
  return (
    <div className="flex min-h-svh p-6">
      <div className="flex max-w-md min-w-0 flex-col gap-4 text-sm leading-loose">
        <div>
          <h1 className="font-medium">Content Manager Placeholder</h1>
          <div className="flex flex-col gap-3">
            <Button variant={"default"}>default Button</Button>
            <Button variant={"secondary"}>secondary Button</Button>
            <Button variant={"outline"}>outline Button</Button>
            <Button variant={"ghost"}>ghost Button</Button>

            <Card>
              <CardContent>
                <h2 className="font-medium">Card Title</h2>
                <p>Card content goes here.</p>
              </CardContent>
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
