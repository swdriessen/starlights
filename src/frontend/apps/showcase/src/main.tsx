import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import { Separator, ThemeProvider } from "ui-framework"
// import App from "./App.tsx"
import "./index.css"
import Preview02Example from "./showcases/preview-02/index.tsx"
import PreviewExample from "./showcases/preview/index.tsx"
import { CharacterOverviewExample } from "./showcases/wip/character-overview-example.tsx"
import WipExample from "./showcases/wip/index.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      {/* <App /> */}
      <CharacterOverviewExample />
      <WipExample />
      <Separator className="my-8" />
      <PreviewExample />
      <Preview02Example />
    </ThemeProvider>
  </StrictMode>
)
