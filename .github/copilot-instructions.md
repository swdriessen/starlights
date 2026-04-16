# Copilot Instructions

These guidelines are tailored to this repository. They consolidate prior guidance, remove duplication, and keep only what’s actionable.

## Project Overview

- **Backend**: .NET 10 Web API (C#) using FastEndpoints in a modular monolith architecture
- **Frontend**: React 19 (TypeScript) with Vite, TanStack Query, React Router, Tailwind CSS, and Shadcn UI
- **Data**: Entity Framework Core 10 with SQL Server
- **Orchestration**: .NET Aspire 13 (AppHost) for local orchestration
- **Testing**: MSTest (Microsoft.Testing.Platform) + AwesomeAssertions + Moq; plus integration and acceptance test projects (Reqnroll)
- **Observability**: Serilog for logging, OpenTelemetry for distributed tracing and metrics

## Repository Layout (high level)

- `src/apps/`: Deployable apps (e.g., the backend Web API)
- `src/aspire/`: Aspire AppHost + ServiceDefaults
- `src/frontend/`: npm workspaces (app + shared UI library)
- `src/modules/`: Business modules (Characters, Elements)
- `src/platform/`: Shared platform + reusable components
- `src/tests/`: Integration and acceptance tests
- `src/source-generators/`: Roslyn source generators + tests

## C# Practices

- Prefer immutability; minimize mutable state. Use explicit access modifiers.
- Async/await for I/O. Log for diagnostics, not control flow.
- Apply SOLID principles. Use DI for all services/abstractions via constructor injection.
- Use records for simple data containers and value objects.
- Enable nullable reference types and rely on them:
  - Use `?` for optional members.
  - Don't add manual null checks for non-nullable DI parameters.
  - Add explicit null checks only when null would cause runtime issues beyond compiler analysis.
- Summary/XML comments for public APIs; use `<inheritdoc />` when implementing documented interfaces.
- Use string interpolation; avoid magic values (prefer constants/enums).
- Prefer pattern matching/switch expressions, LINQ, and collection initializers.
- Dispose `IDisposable` properly (use `using` statements/declarations).
- Follow naming conventions: PascalCase for types/public members, camelCase for local variables/private fields.

## Architecture: Modular Monolith

- Organize by business capability with clear module boundaries:
  - **Elements Module**: Game data (classes, abilities, features, rules)
  - **Characters Module**: Character creation and management
  - **Platform Layer**: Shared infrastructure (hosting, logging, data, eventing)
- Each module follows internal layering:
  - `Domain`: Entities, value objects, aggregates, domain logic
  - `Data`: Repository interfaces and abstractions
  - `Data.EntityFramework`: EF Core configurations, DbContext, migrations
  - `Data.EntityFramework.MigrationService`: Migration worker used by Aspire at startup
  - `Data.EntityFramework.EventProcessing`: Outbox/event processing persistence and workers
  - `Endpoints`: FastEndpoints for API exposure
  - `Integration`: Public contracts for inter-module communication
- Strict boundaries: modules interact via interfaces or domain events; avoid leaking internals.
- Favor internal visibility; expose only what’s necessary.
- Use feature folders/namespaces for related functionality.
- Keep a minimal, stable shared kernel in the Platform layer.
- Apply DDD concepts where beneficial (entities, aggregates, value objects).
- Write integration tests for module interactions and endpoint behavior.

## Aspire 13 (local orchestration)

- **Local Development**:
  - Aspire AppHost entrypoint is `src/aspire/Starlights.AppHost/AppHost.cs`.
  - Use Aspire service discovery for resource connections (SQL Server, backend endpoints).
  - SQL Server container runs on static port `61070` for consistency.
  - Migration workers run at startup (in run mode) and the backend waits for completion.
  - Frontend is started by Aspire as a JavaScript app; it can also be exposed via a Dev Tunnel (explicit start).
  - Monitor via the Aspire dashboard (logs/metrics/traces).

## Entity Framework Core + SQL Server

- **Configuration**: All entities must have explicit `IEntityTypeConfiguration<T>` implementations in respective `*.Data.EntityFramework` projects.
- **Query Performance**:
  - Add indexes for frequently queried columns
  - Use efficient LINQ queries; avoid client evaluation and N+1 queries
  - Project with `.Select()` to load only needed columns
  - Use `AsNoTracking()` for read-only queries
- **Data Transfer**:
  - Load only needed data
  - Avoid cartesian explosion in joins
  - Batch operations where possible
  - Paginate large result sets
- **Runtime**:
  - Disable change tracking when not needed
  - Consider compiled queries for hot paths
  - Use value converters and shadow properties judiciously
- **Caching**: Cache hot data appropriately; plan cache invalidation strategy.
- **Diagnostics**: Enable EF logging; inspect generated SQL; use SQL Profiler/App Insights; measure before optimizing.
- **Migrations**:
  - Use EF Core migrations for schema changes
  - Add migrations in respective module `*.Data.EntityFramework` projects
  - Migration workers run automatically at startup via Aspire
- **General**:
  - Use transactions for multi-step operations
  - Handle transient faults with retry policies
  - Secure connection strings via Aspire service discovery

## FastEndpoints API

- Follow REST principles and REPR (Request, Endpoint, Response) pattern per feature.
- Attribute-free endpoint classes; organize using endpoint groups (e.g., `CharactersGroup`, `ElementsGroup`).
- Secure by default: use policies/roles/claims; `AllowAnonymous` only when needed.
- **Validation**: Create `Validator<T>` for each request (auto-wired by FastEndpoints).
- Use async `Task` handlers; DI via constructor injection.
- Standardize error responses using FastEndpoints built-in error handling.
- API versioning via endpoint groups (e.g., `/api/v1/characters`).
- Enable CORS globally for development; configure appropriately for production.
- **Authentication**: Use cookie-based auth; support GitHub OAuth for identity.
- **Documentation**: Expose OpenAPI spec at `/openapi/v1.json`; interactive docs via Scalar at `/scalar`.
- **Testing**: Prefer integration tests for endpoints using `WebApplicationFactory`; don't create unit tests for endpoint classes.

## React/TypeScript Frontend

- **Monorepo**: Frontend lives in `src/frontend/` and uses npm workspaces.
  - App: `src/frontend/builder-app/`
  - Shared UI library: `src/frontend/libs/ui/` (Radix primitives + shadcn-style patterns)
- **Structure (builder-app)**: Prefer feature folders under `src/` and keep API access centralized.
- **State Management**: Use TanStack Query for server state; React hooks for local state.
- **Routing**: Use React Router 7 with route-based code splitting.
- **Styling**: Tailwind CSS 4; shared UI components should be added to the `@starlights/ui` library when reusable.
- **Forms**: React Hook Form + Zod for validation.
- **API Integration**: Type-safe API calls; centralized error handling; loading states.
- **TypeScript**: Strict mode enabled; proper typing for all components and functions.
- **Environment**:
  - Prefer `import.meta.env.VITE_API_BASE` for the API base URL.
  - In Aspire-run development, the Vite config resolves the base URL from Aspire-injected service discovery env vars (e.g., `services__backend__https__0`).

## Unit Testing (MSTest + AwesomeAssertions + Moq)

- **Structure**:
  - Clear test names: `Class_Method_Scenario` or `Method_WhenCondition_ExpectedBehavior`
  - Use `[TestClass]` and `[TestMethod]` attributes
  - Use `[DataRow]` for parameterized tests
- **Style**:
  - Follow AAA pattern (Arrange, Act, Assert)
  - Use AwesomeAssertions exclusively for assertions
  - Use Moq with strong typing and strict behavior where critical
- **Coverage Requirements** for any new component (class/record/struct):
  - Construction and defaults
  - All public members (methods, properties)
  - Happy path and edge cases
  - Error conditions and validation
  - Do NOT test for `ArgumentNullException` on non-nullable parameters—trust NRT and DI
- **Moq Usage**:
  - Setup using `.Setup(...)` with lambda expressions
  - Match parameters with `It.IsAny<T>()` or `It.Is<T>(predicate)`
  - Verify calls with `.Verify(...)` as needed
  - Use `MockBehavior.Strict` when order and exact calls matter
  - Reset mocks between tests if sharing instances
- **Lifecycle**: Use `[TestInitialize]` and `[TestCleanup]` for setup/teardown.
- **Integration Tests**:
  - Use `WebApplicationFactory` pattern for endpoint testing
  - Test module interactions and boundary contracts
  - Verify database state changes where applicable
- **Acceptance Tests**:
  - Acceptance tests live under `src/tests/acceptance/` and use Reqnroll (BDD).
  - Keep step definitions focused and reuse the integration harness where possible.
- **Always build and run the full test suite before considering work complete**.
  - Specifying a project for 'dotnet test' should be via '--project'.

## Repository-Specific Rules

- **Characters Module**: When adding domain entities, also create an `IEntityTypeConfiguration<T>` in `Modules.Characters.Data.EntityFramework/Configurations/`.
- **Elements Module**: When adding domain entities or element components, also create an `IEntityTypeConfiguration<T>` in `Modules.Elements.Data.EntityFramework/Configurations/`.
- **Domain Events**: Use the platform's domain event publisher for inter-module communication; avoid direct module-to-module references.
- **Value Objects**: Implement as records with proper equality semantics; configure as owned entities or with value converters in EF Core.
- **Aggregates**: Keep aggregate roots as entry points for all modifications; enforce invariants within the aggregate.

## Source Generators

- Source generators live under `src/source-generators/` and are built as analyzers.
- When adding generator tests, reference the generator project as an analyzer (see `OutputItemType="Analyzer"` pattern in the generator test project).

## Code Generation Guidelines

- When generating endpoint code:
  - Create request/response record types
  - Add validator class for requests
  - Implement endpoint with proper group assignment
  - Include XML documentation
  - Don't generate unit tests for endpoint classes (integration tests only)
- When generating domain entities:
  - Include proper encapsulation (private setters, factory methods)
  - Add domain validation in constructors or methods
  - Create corresponding `IEntityTypeConfiguration<T>` class
  - Generate unit tests covering construction, validation, and behavior
- When generating services:
  - Define interface first
  - Implement with DI via constructor
  - Add proper logging at appropriate levels
  - Generate unit tests with mocked dependencies

## Pull Request Checklist

- [ ] Build succeeds locally with no warnings
- [ ] All tests pass (`dotnet test`)
- [ ] Code coverage expectations met (configured in `.runsettings`)
- [ ] Aspire manifests updated (local + Azure) and aligned
- [ ] Secrets handled via Azure Key Vault (production) or Aspire service discovery (local)
- [ ] EF Core migrations added/updated for schema changes
- [ ] Entity type configurations created for new entities (Characters/Elements modules)
- [ ] Endpoints validated with integration tests
- [ ] Security policies and validation in place for new endpoints
- [ ] Frontend types/API clients updated for backend changes
- [ ] Documentation updated (README, XML comments, inline comments where needed)
- [ ] No hardcoded secrets or configuration values
- [ ] Proper error handling and logging added
- [ ] Performance considerations addressed (indexes, query optimization, caching)

## Additional Best Practices

- **Logging**: Use structured logging with Serilog; include correlation IDs for distributed tracing.
- **Error Handling**: Use problem details for API errors; handle exceptions at appropriate boundaries.
- **Configuration**: Use strongly-typed options pattern; validate configuration at startup.
- **Security**: Follow principle of least privilege; validate all inputs; sanitize outputs.
- **Performance**: Profile before optimizing; use caching judiciously; consider async throughout.
- **Documentation**: Keep README current; document architectural decisions; comment complex logic only.
- **Git Workflow**: Use feature branches; write clear commit messages; keep PRs focused and reviewable.

## Commit Messages

Use Conventional Commit Messages by prefixing a single line commit message, do not include a full summary.

The following prefixes are valid: feat, fix, refactor, perf, style, test, docs, build, ops, chore.

- **feat**: Commits that add, adjust or remove a new feature to the API or UI
- **fix**: Commits that fix an API or UI bug of a preceded feat commit
- **refactor**: Commits that rewrite or restructure code without altering API or UI behavior
- **perf**: Commits are special type of refactor commits that specifically improve performance
- **style**: Commits that address code style (e.g., white-space, formatting, missing semi-colons) and do not affect application behavior
- **test**: Commits that add missing tests or correct existing ones
- **docs**: Commits that exclusively affect documentation
- **build**: Commits that affect build-related components such as build tools, dependencies, project version, ...
- **ops**: Commits that affect operational aspects like infrastructure (IaC), deployment scripts, CI/CD pipelines, backups, monitoring, or recovery procedures, ...
- **chore**: Commits that represent tasks like initial commit, modifying .gitignore, ...
