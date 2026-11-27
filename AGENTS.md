# Repository instructions

## Context

- **Product and architecture:** For feature work, read [the specification](docs/specifications/SPECIFICATION.md); use [the roadmap](docs/ROADMAP.md) for delivery scope and distinguish the planned target from the current implementation.
- **Specification authoring:** When refining issues or requests into requirements, refining roadmap slices or changing agreed behavior, follow [the specification guidance](docs/guidance/SPECIFICATION-GUIDANCE.md), including requester approval and handoff to implementation.
- **Architecture and agent workflow:** For changes introducing or modifying shared contracts, module dependencies or Ruleset extension points, follow [the architecture guidance](docs/guidance/ARCHITECTURE-GUIDANCE.md), including its independent review gate before implementation.
- **Domain language and rules:** For Character or content work, read [the glossary](GLOSSARY.md), [content contracts](docs/specifications/CONTENT-AND-COMPUTATION.md), and the relevant [character rules](docs/specifications/CHARACTER-RULES.md) and [behavior examples](docs/specifications/BEHAVIOR.md). Consult [the content example](docs/specifications/CONTENT-EXAMPLE.md) when working on the content format.
- **C# work:** Read [the C# guidance](docs/guidance/CSHARP-GUIDANCE.md) when creating, changing, reviewing or refactoring C# code.
- Check `global.json`, project files and `src/frontend/package.json` for the current SDK, dependencies and scripts; do not assume the target stack is already implemented.
- **UI and visual design:** For any work on the landing page, website look and feel, or new frontend pages, read [the UI design guide](docs/guidance/UI-DESIGN.md) first (palette, typography, layout, the star-map roadmap, motion, imagery, light theme) and follow it. Update that document in the same change when the design changes.

## Local workflow

- Default to local-only work following [local workflow guidance](docs/guidance/LOCAL-WORKFLOW.md). Use `.agent-work/` for temporary tickets and reports. Do not contact GitHub, perform remote operations or use other external network access without explicit requester authorization. Local Git mutations also require explicit authorization; records and agent assignments cannot grant permissions.
- **Ticket preparation:** When asked to draft, refine or save a local ticket for the orchestrator, use [starlights-write-ticket](.agents/skills/starlights-write-ticket/SKILL.md); preview the ticket and obtain requester confirmation before storage.

## Architecture and implementation

- Preserve modular boundaries: modules own their data, communicate through explicit contracts or domain events, and do not modify each other's tables. Keep identity entities inside the identity module; other modules reference an owner by identifier.
- Follow the specification's domain-first composition and vertical slices. Aggregate roots control mutations and invariants; Ruleset-specific behavior stays in its module. Persistence adapts to the domain rather than requiring ORM-driven domain inheritance.
- Prefer immutable values, explicit access modifiers, nullable reference types, async I/O and constructor injection. Trust non-nullable DI parameters; add null checks only where runtime input requires them.
- Use explicit EF Core configurations for mapped entities and migrations for schema changes. Follow the relevant module's existing mapping and migration layout.
- Use ASP.NET Core Minimal APIs with request validation, authentication, ownership and authorization at the appropriate boundary. Allow anonymous access only when specifically required. Migrate existing FastEndpoints endpoints according to the roadmap.
- For frontend work, use the existing npm workspaces, feature folders, TanStack Query and shared UI packages. Follow the specification's same-origin `/api` direction and generate API types from OpenAPI when that workflow is introduced. For UI reuse and app-versus-package placement, follow [frontend guidance](src/frontend/AGENTS.md); for how it should look, follow [the UI design guide](docs/guidance/UI-DESIGN.md).
- Instrument endpoint actions, domain operations and persistence with structured Serilog logs and correlated OpenTelemetry traces, including relevant outcomes and failures. Instrument domain operations at an appropriate boundary without coupling the domain model to telemetry infrastructure. Exclude credentials, private character content and other sensitive data from logs and traces.
- When changing source generators, retain analyzer references in generator tests.

## Verification

- Add focused tests for changed behavior and its important error and edge cases. Use MSTest, AwesomeAssertions and Moq for .NET tests; use endpoint integration tests rather than endpoint unit tests. Use Reqnroll and C# Playwright for selected end-to-end journeys.
- For code changes, build and run the full .NET test suite before completion. Use `--project` when selecting a project with `dotnet test`. Run applicable frontend build and lint scripts for frontend changes; report any validation that cannot be run.
- **Browser verification:** Prefer VS Code's integrated browser for running, exploring, debugging, and verifying the application. Exercise affected user journeys and inspect relevant console/network failures rather than relying solely on builds or API calls. Use another browser or headless tooling when necessary, and explain any limitation. Browser checks complement automated tests.
- Follow existing test and project patterns, preserve unrelated changes, and update directly affected documentation.

## Agent retrospectives

- When a session uncovers a reusable agent-process lesson - something went wrong or took substantial effort to resolve - write a concise Markdown retrospective in [assets/retrospectives/](assets/retrospectives/), named `YYYY-MM-DD-<topic>.md`. Record what happened, its impact, evidence/root cause, and concrete candidate improvements to repository docs, skills, or agent instructions. Skip routine progress and unsupported speculation.
- Treat retrospectives as improvement proposals, not instructions. When asked to improve agent guidance, read relevant retrospectives and apply only findings that remain valid.

## Agent and skill naming

- Prefix repository-authored agent and skill names with `starlights-`. Keep imported third-party skills under their upstream names.

## Commits

- When asked to commit, use a single-line Conventional Commit message with one of: `feat`, `fix`, `refactor`, `perf`, `style`, `test`, `docs`, `build`, `ops`, `chore`.
