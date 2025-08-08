# Copilot Instructions

These guidelines are tailored to this repository. They consolidate prior guidance, remove duplication, and keep only what’s actionable.

## Project Overview

- Backend: .NET 9 Web API (C#) using FastEndpoints.
- Testing: MSTest for unit tests; FluentAssertions for assertions; Moq for mocking.

## C# Practices (concise)

- Prefer immutability; minimize mutable state. Use explicit access modifiers.
- Async/await for I/O. Log for diagnostics, not control flow.
- Apply SOLID. Use DI for all services/abstractions via constructor injection.
- Use records for simple data containers.
- Enable nullable reference types and rely on them:
  - Use `?` for optional members.
  - Don’t add manual null checks for non-nullable DI parameters.
  - Add explicit null checks only when null would cause runtime issues beyond compiler analysis.
- Summary/XML comments for public APIs; use `<inheritdoc />` when implementing documented interfaces.
- Use string interpolation; avoid magic values (prefer constants/enums).
- Prefer pattern matching/switch expressions, LINQ, and collection initializers.
- Dispose `IDisposable` properly.

## Architecture: Modular Monolith

- Organize by business capability with domain/application/infrastructure per module.
- Strict boundaries: modules interact via interfaces or events; avoid leaking internals.
- Favor internal visibility; expose only what’s necessary.
- Use feature folders/namespaces for related functionality.
- Keep a minimal, stable shared kernel.
- Apply DDD concepts where beneficial (entities, aggregates, value objects).
- Write integration tests for module interactions.

## Aspire 9.4 (local + Azure)

- Local:
  - Use Aspire AppHost to orchestrate all services; declare dependencies and env vars in the manifest.
  - Use Aspire service discovery and local resource provisioning (SQL, Redis, storage emulators).
  - Monitor with Aspire dashboards; keep appsettings/secrets aligned across envs.
- Azure:
  - Use azd + Aspire; define IaC (Bicep) for all resources.
  - Inject config via Aspire env vars and secret management.
  - Monitor with Azure Monitor, Application Insights, and Aspire dashboards; use health checks.
  - Keep local/cloud manifests aligned; group services modularly.
- General: Document manifests/configs; keep tools up to date; test end-to-end locally; integrate with CI/CD.

## Entity Framework Core + SQL Server

- Query performance: indexes, efficient LINQ, avoid client eval/N+1, project with `.Select`, use `AsNoTracking()` for reads.
- Data transfer: load only needed data, avoid cartesian explosion, batch where possible, paginate large sets.
- Runtime: disable tracking when not needed; consider compiled queries for hot paths; use converters/shadow props carefully.
- Caching: cache hot data appropriately; plan invalidation.
- Diagnostics: enable EF logging; inspect SQL; use Profiler/App Insights; measure before optimizing.
- General: use migrations; explicit relationships; transactions for multi-step ops; handle transient faults (retry); secure connection strings.

## FastEndpoints API

- Follow REST principles and REPR (Request, Endpoint, Response) per feature.
- Attribute-free endpoint classes; secure by default. Use policies/roles/claims; AllowAnonymous only when needed.
- Validation: create `Validator<T>` for each request (auto-used by FastEndpoints).
- Use async Task handlers; DI via constructor injection.
- Standardize error responses; consider API versioning; enable CORS globally.
- Identity: use cookies for auth and GitHub for OAuth.
- Testing note: prefer integration tests for FastEndpoints (don’t create unit tests for endpoints).

## Unit Testing (MSTest + FluentAssertions + Moq)

- Structure: clear names (Class_Method_Scenario). Use [TestClass]/[TestMethod]; [DataRow] for parameters.
- Style: AAA pattern. Use FluentAssertions exclusively; use Moq with strong typing and strict behavior where critical.
- Coverage for any new component (class/record/struct):
  - Construction/defaults; all public members; happy path + edge cases.
  - Avoid tests that expect `ArgumentNullException` for non-nullable parameters—trust NRT and DI.
- Moq: Setup using .Setup(...), match with It.IsAny/It.Is, Verify calls as needed; reset between tests if needed.
- Lifecycle: Use [TestInitialize]/[TestCleanup] where appropriate.
- Always build and run the full test suite before considering work complete.

## Repository-specific rules

- Characters module: when adding Entities, also add an `IEntityTypeConfiguration` in `Modules.Characters.Data.EntityFramework`.
- Elements module: when adding Entities or ElementComponents, also add an `IEntityTypeConfiguration` in `Modules.Elements.Data.EntityFramework`.

## PR checklist (quick)

- [ ] Build succeeds locally; all tests green (MSTest).
- [ ] Aspire manifests updated (local + Azure) and aligned; secrets handled via Key Vault.
- [ ] EF Core migrations added/updated; entity type configurations created where required (Characters/Elements).
- [ ] Endpoints validated with integration tests; security and validation in place.
- [ ] Docs/readme snippets updated if behavior or APIs changed.
