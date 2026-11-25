import { StrictMode, Suspense, lazy } from "react"
import { createRoot } from "react-dom/client"

import { ThemeProvider } from "ui-framework"
import App from "./App.tsx"
import "./index.css"

// Landing variants are separate pages (/v1, /v2, ...), loaded only when visited.
const pages: Record<string, ReturnType<typeof lazy>> = {
  "/v1": lazy(() => import("./variants/v1/V1.tsx")),
  "/v2": lazy(() => import("./variants/v2/V2.tsx")),
  "/v3": lazy(() => import("./variants/v3/V3.tsx")),
  "/v4": lazy(() => import("./variants/v4/V4.tsx")),
  "/v5": lazy(() => import("./variants/v5/V5.tsx")),
}

const path = window.location.pathname.replace(/\/+$/, "") || "/"
const Page = pages[path]

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider defaultTheme="dark">
      {Page ? (
        <Suspense fallback={null}>
          <Page />
        </Suspense>
      ) : (
        <App />
      )}
    </ThemeProvider>
  </StrictMode>
)
