import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { env as nodeEnv } from "process";
import { defineConfig, loadEnv } from "vite";

export default defineConfig(({ mode }) => {
  const viteEnv = loadEnv(mode, process.cwd(), "");

  const apiHttps = viteEnv.VITE_API_HTTPS || nodeEnv.services__backend__https__0 || "N/A";
  const apiHttp = viteEnv.VITE_API_HTTP || nodeEnv.services__backend__http__0 || "N/A";
  const apiBase = viteEnv.VITE_API_BASE || nodeEnv.services__backend__http__0 || nodeEnv.services__backend__https__0 || "http://localhost:5240";

  console.log("===== RESOLVED ENVIRONMENT =====");
  console.log("NODE_ENV =", nodeEnv.NODE_ENV);
  console.log("HOST =", nodeEnv.HOST);
  console.log("PORT =", nodeEnv.PORT);
  console.log("API HTTPS =", apiHttps);
  console.log("API HTTP =", apiHttp);
  console.log("API BASE =", apiBase);
  console.log("================================");

  return {
    plugins: [react(), tailwindcss()],
    resolve: {
      tsconfigPaths: true,
    },
    server: {
      port: parseInt(nodeEnv.PORT || "55052"),
      open: true,
      proxy: {
        "/api": {
          target: apiBase,
          changeOrigin: false,
          secure: !["localhost", "127.0.0.1", "[::1]"].includes(new URL(apiBase).hostname),
        },
      },
    },
    define: {
      "import.meta.env.VITE_API_HTTPS": JSON.stringify(apiHttps),
      "import.meta.env.VITE_API_HTTP": JSON.stringify(apiHttp),
      "import.meta.env.VITE_API_BASE": JSON.stringify(""),
    },
  };
});
