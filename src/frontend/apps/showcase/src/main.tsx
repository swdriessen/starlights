import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import { ThemeProvider } from "ui-framework"
// import App from "./App.tsx"
import "./index.css"
import Preview02Example from "./showcases/preview-02/index.tsx"
import PreviewExample from "./showcases/preview/index.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      {/* <App /> */}
      <PreviewExample />
      <Preview02Example />
    </ThemeProvider>
  </StrictMode>
)
