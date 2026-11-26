# Frontend readiness for F01

Updated: 2026-11-25

## Current status

- Local Aspire verification is complete, as accepted by the user.
- Developer-tunnel and private-network verification is intentionally deferred.
- This frontend pass focused on standardizing the builder on `packages/ui-framework` and its shadcn/Base UI components before F01.

## Completed

- Kept the npm workspace layout and `@starlights/api-client` package.
- Updated the shared UI package's supporting dependencies: `lucide-react`, `react-resizable-panels`, `@types/node`, and `@types/react`.
- Migrated builder consumers to `@starlights/ui-framework`, moved builder-specific sidebar/navigation compositions and legacy styles into the app, and removed the builder-local UI wrappers and obsolete `packages/ui` workspace.
- Removed builder-local Radix dependencies and stale aliases/configuration. The builder now builds the shared UI package before starting Vite.
- Fixed Base UI integration details found during verification: nullable Select values, AlertDialog action/close behavior and optional button variants, and semantic anchors styled with `buttonVariants`.
- Builder now uses `ui-framework`'s shared `ThemeProvider` and `useTheme`, retaining its dark default and storage key; the shared provider owns the D-key light/dark shortcut.
- Recorded the migration in `apps/builder-app/.migration/`. No specification or roadmap documents were changed.

## Validation

- `npm install` from `src/frontend`: passed; lockfile regenerated. npm reported 8 audit findings (6 moderate, 2 high); these were not remediated in this UI migration.
- Shared UI build and builder TypeScript/Vite production build: passed.
- Targeted lint on changed builder files and the changed shared AlertDialog wrapper: passed. The builder build and targeted lint on the shared-provider imports also passed after switching the builder to the shared theme provider.
- Full builder lint previously reported issues in the local theme provider and character-details page; the local provider is now removed, but full lint has not been rerun. Full shared-UI lint still reports existing `react/only-export-components` diagnostics in other wrappers.
- Browser smoke checks with mocked session/file API: builder sidebar toggles and dropdown menu, landing navigation menu, and file AlertDialog open/cancel/action all worked without new console errors.
- A follow-up browser check of the D-key toggle was blocked by the session endpoint returning 502 and the browser automation connection timing out.
- Vite emitted its existing config-loader warning and a large-chunk warning during the original migration build.

## Local Aspire verification

- The complete local AppHost started with SQL Server and the storage emulator healthy; the migration workers completed successfully.
- Real browser sign-in through the builder's same-origin Vite proxy succeeded after the HTTPS proxy fix.
- The user accepted local Aspire verification as complete. A separate full-graph isolated run and startup-failure injection were not performed.
- Developer-tunnel and private-network verification remain deferred and are not included in this acceptance.

## Notes before F01

- Use `@starlights/ui-framework` for shared controls and keep feature-specific composition in the builder app; keep API calls in the existing API client/query structure.
- The builder has no remaining Radix wrappers or direct Radix imports. Transitive Radix packages remain through `cmdk` and `vaul`; their implementations were intentionally left alone.
- Preserve the flagged Base UI behavior differences during feature work: Tabs uses manual activation, Navigation Menu opens with the Base UI delay, and menu item close behavior differs for some checkbox/radio items.
