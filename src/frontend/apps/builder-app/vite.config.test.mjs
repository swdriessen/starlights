import assert from "node:assert/strict";
import { test } from "node:test";
import { fileURLToPath } from "node:url";
import { loadConfigFromFile } from "vite";

const configFile = fileURLToPath(new URL("./vite.config.ts", import.meta.url));
const keys = ["VITE_API_BASE", "services__backend__https__0", "services__backend__http__0"];

async function proxyTarget(t, values) {
  const original = Object.fromEntries(keys.map((key) => [key, process.env[key]]));
  t.after(() => {
    for (const key of keys) {
      if (original[key] === undefined) delete process.env[key];
      else process.env[key] = original[key];
    }
  });
  process.env.VITE_API_BASE = values.VITE_API_BASE ?? "";
  for (const key of keys.slice(1)) {
    if (values[key] === undefined) delete process.env[key];
    else process.env[key] = values[key];
  }
  const result = await loadConfigFromFile({ command: "serve", mode: "test" }, configFile);
  assert.ok(result);
  return result.config.server.proxy["/api"].target;
}

test("prefers Aspire HTTPS when both backend endpoints are available", async (t) => {
  const target = await proxyTarget(t, {
    services__backend__https__0: "https://localhost:7246",
    services__backend__http__0: "http://localhost:5240",
  });
  assert.equal(target, "https://localhost:7246");
});

test("respects an explicit proxy target", async (t) => {
  const target = await proxyTarget(t, {
    VITE_API_BASE: "http://localhost:9000",
    services__backend__https__0: "https://localhost:7246",
  });
  assert.equal(target, "http://localhost:9000");
});

test("uses HTTP when Aspire exposes no HTTPS endpoint", async (t) => {
  const target = await proxyTarget(t, {
    services__backend__http__0: "http://localhost:5240",
  });
  assert.equal(target, "http://localhost:5240");
});
