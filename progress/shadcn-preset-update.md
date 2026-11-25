# Shared UI preset update

## Result

Applied `b5defF3dg` to the [shared UI framework](../src/frontend/packages/ui-framework):
Base UI, Mira styling, Zinc neutrals, Sky primary, Teal charts, Inter Variable,
inherited heading font, default radius, and subtle/default menus.

Merged style changes into 47 of the 55 installed registry components. The other
eight needed no style changes. Kept public APIs, imports, theme infrastructure,
the existing `cn` helper, and local keyboard-focus adjustments.

## Executed verification

- `npx shadcn@latest preset resolve --json` returned `b5defF3dg` with no fallbacks.
- From the frontend workspace: `npm run typecheck -w ui-framework`,
  `npm run build -w ui-framework`, and `npm run build -w builder-app` passed.
- Parsed TypeScript AST comparison found only 215 styling-string changes across
  the 47 changed components, with no code-structure, import, or signature changes.
- Library lint still reports the same 10 pre-existing
  `react(only-export-components)` errors. No additional lint errors appeared.
- Inspected the running character collection in both themes: Inter Variable and
  the expected Sky tokens were active, Mira cards had a 10px computed radius,
  large buttons measured 32px, and the desktop viewport had no horizontal overflow.
  Restored the original dark theme afterward.
- The displayed collection was already loaded. A pre-existing request to a
  retired local backend endpoint still failed; API workflows were not re-tested
  through that browser page as part of this styling change.
- `dotnet test --solution Starlights.slnx --no-restore` failed during build with
  MSB3027 because the running backend held output DLLs open. No shared processes
  were stopped or restarted.
- `dotnet test --solution Starlights.slnx --no-build --no-restore` passed using
  existing binaries: Passed: 646, Failed: 0, Skipped: 16.
- Existing Vite native-config and bundle-size warnings remain unchanged.

## Agentic workflow learnings

1. A preset includes component density and shape, not just colors and fonts.
   Verify rendered controls and the resolved preset, rather than treating a
   configuration edit as completion.
2. CLI initialization can also replace utilities and add dependencies. Check
   those side effects and retain local helpers when the preset does not need
   their replacement.
3. Registry JSON is not installation-ready source. Apply the CLI's own import,
   icon, font, and menu transformations before a three-way component merge.
4. Normalize formatting in scratch merge inputs to separate presentation noise
   from genuine local customizations. Refuse all source writes until every
   conflict has an explicit, checked resolution.
5. Compare behavior separately from presentation. An AST-level style-only check,
   consumer builds, and live computed styles provide complementary evidence.
6. In a shared development environment, locked application outputs are not a
   reason to kill processes. Report the failed rebuild separately from a
   successful run against unchanged existing test binaries.
