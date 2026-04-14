import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import path from "path"
import { defineConfig } from "vite"

const externalDeps = [
  "react",
  "react-dom",
  "react/jsx-runtime",
  "react/jsx-dev-runtime",
  "recharts",
]

const isExternalLibrary = (id: string) =>
  externalDeps.includes(id) ||
  id.startsWith("@base-ui/react/") ||
  id === "@base-ui/react" ||
  id.startsWith("use-sync-external-store/") ||
  id === "use-sync-external-store"

export default defineConfig({
  plugins: [react(), tailwindcss()],
  build: {
    chunkSizeWarningLimit: 1600,
    lib: {
      entry: path.resolve(__dirname, "src/index.ts"),
      formats: ["es"],
      fileName: "index",
    },
    rolldownOptions: {
      external: isExternalLibrary,
      output: {
        codeSplitting: true,
      },
    },
    rollupOptions: {
      external: isExternalLibrary,
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
