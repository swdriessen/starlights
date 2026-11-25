# UI Framework

Preset: `b5defF3dg` (`--preset b5defF3dg`).

- Base UI primitives with Mira component styling.
- Zinc neutrals, Sky primary, Teal charts, and Inter Variable typography.
- Shared light and dark tokens live in [index.css](./index.css).

Run shadcn commands from this package directory. Verify the active preset with
`npx shadcn@latest preset resolve --json`.

When changing presets, merge upstream component styles rather than overwriting
local behavior. Preserve the existing `cn` helper, public exports, and local
keyboard-focus adjustments.
