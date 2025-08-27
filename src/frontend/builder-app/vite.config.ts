import { defineConfig } from "vite";
import { env } from "process";
import react from "@vitejs/plugin-react";
import path from "path";
import tailwindcss from "@tailwindcss/vite";

console.log("===== environment =====");
console.log("NODE_ENV =", env.NODE_ENV);
console.log("HOST =", env.HOST);
console.log("PORT =", env.PORT);
console.log("API HTTPS =", env.services__backend__https__0);
console.log("API HTTP =", env.services__backend__http__0);
console.log("=======================");

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: parseInt(env.PORT || "55052"),
    open: true,
  },
});
