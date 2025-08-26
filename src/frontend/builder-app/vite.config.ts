import { defineConfig } from "vite";
import { env } from "process";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(env.PORT || "55052"),
    open: true,
  },
});
