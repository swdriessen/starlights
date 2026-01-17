import path from "path";
import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";
import { env } from "process";

console.log("BACKEND_HTTPS =", env.BACKEND_HTTPS);
console.log("PORT =", env.PORT);

const CONTENT_API_ENDPOINT = env.BACKEND_HTTPS || "https://localhost:12345";
const PORT = env.PORT || "12345";

console.log("CONTENT_API_ENDPOINT =", CONTENT_API_ENDPOINT);
console.log("PORT =", PORT);

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: parseInt(PORT),
    open: true,
  },
  define: {
    "import.meta.env.VITE_CONTENT_API_ENDPOINT": JSON.stringify(CONTENT_API_ENDPOINT),
  },
});
