# Frontend guidance

- Before adding a UI primitive, provider, hook, theme behavior, or design token, check `packages/ui-framework` and its public exports. Reuse shared implementations in apps instead of copying them.
- Keep app-specific routes, workflows, data access, and page composition in the owning app. Promote behavior to `ui-framework` when it is genuinely shared.
- For app theming, use the shared `ThemeProvider` and `useTheme` exports and mount the provider at the app root. Theme persistence and the D-key shortcut belong to that shared provider; avoid app-local duplicates.
- Import through the package name declared by the app and its public exports; avoid importing implementation files directly from `packages/ui-framework/src`.
