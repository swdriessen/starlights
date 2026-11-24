import { execFileSync } from "node:child_process";

const source = new URL(process.argv[2] ?? "http://localhost:5240/openapi/v1.json");
// The generator's TypeScript 5 peer stays isolated from the application's TypeScript 7.
execFileSync(process.execPath, [
  process.env.npm_execpath,
  "exec", "--yes", "--package=openapi-typescript@7.13.0", "--package=typescript@5.9.3", "--",
  "openapi-typescript", source.href, "--output", "src/generated/schema.d.ts",
], { stdio: "inherit" });
