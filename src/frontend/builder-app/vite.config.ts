import { defineConfig } from "vite";
import { env } from "process";
import react from "@vitejs/plugin-react";

console.log("===== environment =====");
console.log("NODE_ENV =", env.NODE_ENV);
console.log("HOST =", env.HOST);
console.log("PORT =", env.PORT);
console.log("API HTTPS =", env.services__backend__https__0);
console.log("API HTTP =", env.services__backend__http__0);
console.log("=======================");

export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(env.PORT || "55052"),
    open: true,
  },
});
