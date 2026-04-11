import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import { ThemeProvider } from "ui-framework"
import App from "./App.tsx"
import "./index.css"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      <App />
    </ThemeProvider>
  </StrictMode>
)
