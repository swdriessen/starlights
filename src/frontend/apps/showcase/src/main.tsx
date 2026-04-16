import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import { Separator, ThemeProvider } from "ui-framework"
// import App from "./App.tsx"
import "./index.css"
import {
  CharacterOverviewExample,
  ExampleContainer,
} from "./showcases/components/character-overview-example.tsx"
import Preview02Example from "./showcases/preview-02/index.tsx"
import PreviewExample from "./showcases/preview/index.tsx"
import WipExample from "./showcases/wip/index.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      {/* <App /> */}
      <ExampleContainer title="Character Collection Items">
        <CharacterOverviewExample />
      </ExampleContainer>

      <ExampleContainer title="Campaign Cards">
        <WipExample />
      </ExampleContainer>

      <ExampleContainer title="Preview Examples">
        <PreviewExample />
      </ExampleContainer>

      <ExampleContainer title="Preview 2 Examples">
        <Preview02Example />
      </ExampleContainer>
    </ThemeProvider>
  </StrictMode>
)
