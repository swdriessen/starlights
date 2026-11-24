# Repository instructions

## Context

- **Product and architecture:** For feature work, read [the specification](docs/SPECIFICATION.md); use [the roadmap](docs/ROADMAP.md) for delivery scope and distinguish the planned target from the current implementation.
- **Domain language and rules:** For Character or content work, read [the glossary](GLOSSARY.md), [content contracts](docs/CONTENT-AND-COMPUTATION.md), and the relevant [character rules](docs/CHARACTER-RULES.md) and [behavior examples](docs/BEHAVIOR.md). Consult [the content example](docs/CONTENT-EXAMPLE.md) when working on the content format.
- Check `global.json`, project files and `src/frontend/package.json` for the current SDK, dependencies and scripts; do not assume the target stack is already implemented.

## Architecture and implementation

- Preserve modular boundaries: modules own their data, communicate through explicit contracts or domain events, and do not modify each other's tables. Keep identity entities inside the identity module; other modules reference an owner by identifier.
- Follow the specification's domain-first composition and vertical slices. Aggregate roots control mutations and invariants; Ruleset-specific behavior stays in its module. Persistence adapts to the domain rather than requiring ORM-driven domain inheritance.
- Keep each C# class in its own file. Add XML summaries to public types and members; use `<inheritdoc />` when implementing documented interfaces.
- Prefer immutable values, explicit access modifiers, nullable reference types, async I/O and constructor injection. Trust non-nullable DI parameters; add null checks only where runtime input requires them.
- Use explicit EF Core configurations for mapped entities and migrations for schema changes. Follow the relevant module's existing mapping and migration layout.
- Use ASP.NET Core Minimal APIs organized by feature, with request validation, authentication, ownership and authorization at the appropriate boundary. Allow anonymous access only when specifically required. Migrate existing FastEndpoints endpoints according to the roadmap.
- For frontend work, use the existing npm workspaces, feature folders, TanStack Query and shared UI packages. Follow the specification's same-origin `/api` direction and generate API types from OpenAPI when that workflow is introduced.
- Instrument endpoint actions, domain operations and persistence with structured Serilog logs and correlated OpenTelemetry traces, including relevant outcomes and failures. Instrument domain operations at an appropriate boundary without coupling the domain model to telemetry infrastructure. Exclude credentials, private character content and other sensitive data from logs and traces.
- When changing source generators, retain analyzer references in generator tests.

## Verification

- Add focused tests for changed behavior and its important error and edge cases. Use MSTest, AwesomeAssertions and Moq for .NET tests; use endpoint integration tests rather than endpoint unit tests. Use Reqnroll and C# Playwright for selected end-to-end journeys.
- For code changes, build and run the full .NET test suite before completion. Use `--project` when selecting a project with `dotnet test`. Run applicable frontend build and lint scripts for frontend changes; report any validation that cannot be run.
- Follow existing test and project patterns, preserve unrelated changes, and update directly affected documentation.

## Commits

- When asked to commit, use a single-line Conventional Commit message with one of: `feat`, `fix`, `refactor`, `perf`, `style`, `test`, `docs`, `build`, `ops`, `chore`.
