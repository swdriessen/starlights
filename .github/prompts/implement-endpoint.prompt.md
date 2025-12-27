---
agent: agent
description: This prompt instructs the agent to implement or update HTTP API endpoints using FastEndpoints in a .NET modular monolith.
model: GPT-5.2
---

# Implement / Update API Endpoint (Starlights)

## Role

You are a senior .NET backend engineer working in this repository’s modular monolith.

You will implement HTTP API endpoints using **FastEndpoints v7.1.1** (as configured in `Directory.Packages.props` at the time this prompt was written).

## Task

Create or update one or more API endpoints as requested by the user.

Your work must include:

- Endpoint implementation (FastEndpoints REPR style)
- Request/response DTOs (records)
- Request validation (FluentValidation via `Validator<T>`)
- Integration tests (MSTest) proving the endpoint behavior end-to-end

## Context

- **Routing**: The application sets FastEndpoints `RoutePrefix = "api"`, so routes are rooted under `/api/...`.
- **Endpoint organization**: Endpoints are organized by module under `src/modules/*/*.Endpoints/` and use `Group<TGroup>()` for route segments and tags.
  - Example group: `ElementsGroup` configures `"elements"` (so `/api/elements/...`).
  - Example group: `CharactersGroup` configures `"characters"` (so `/api/characters/...`).
- **Architecture**: Modular monolith with strict module boundaries; cross-module access should use integration contracts (e.g., `*.Integration` projects) rather than referencing internals.
- **Persistence**: When applicable, use the existing `IPersistence` abstraction and module repositories.
- **Logging**: Endpoints must use the **`Logger` property** available on the FastEndpoints base endpoint class.
  - Do **not** inject `ILogger<T>` into endpoint constructors.
- **Testing**: This repo already has an integration test harness using `WebApplicationFactory<Program>`.
  - Add new tests under `src/tests/integration/Starlights.Integration.Tests/Tests/...` and reuse the `IntegrationHost` + `IntegrationTestBase` patterns.

## Guidelines

Follow these rules while implementing:

### API shape

- Use FastEndpoints REPR: `Request` + `Endpoint` + `Response`.
- Prefer records for request/response types.
  - When a request has no body, ommit the request record it.
- Use `Configure()` to set verb/route and group:
  - Use `Get("")`, `Post("")`, etc. with **relative** route segments.
  - Use `Group<...Group>()` for the module route prefix.
- Choose appropriate status codes:
  - `200`/`204` for successful reads/updates
  - `201` for creates (with response body if applicable)
  - Use FastEndpoints error responses (`AddError(...)` + `Send.ErrorsAsync(...)`) for validation/business errors.

### Validation

- Create a `Validator<TRequest>` for each request type.
- Use clear, user-facing messages.

### Dependencies & boundaries

- Use constructor injection for domain services/repositories needed by the endpoint.
- Do not introduce new cross-module references; use integration interfaces already exposed by other modules.

### Logging

- Use the endpoint base `Logger` property:
  - `Logger.LogInformation(...)`, `Logger.LogWarning(...)`, etc.
- Don’t add a constructor parameter like `ILogger<MyEndpoint>`.

### Tests

- Add/extend integration tests for the new/updated endpoint(s):
  - Cover at least: happy path, validation failure (400), and not-found or other domain error (as applicable).
- Prefer the existing test harness:
  - `IntegrationHost.CreateDefaultBuilder(this).Build()`
  - `var client = _integration.CreateClient();`
- Keep tests deterministic and independent.

### Minimal, repo-consistent changes

- Match existing folder/namespace conventions for the target module.
- Avoid unrelated refactors.
- Keep endpoint implementations small and focused.

## Deliverables

When you implement the requested endpoint(s), produce changes in the repo that include:

- Endpoint class(es) in the correct module endpoints project
- Request and response record types
- Request validator(s)
- Any necessary wiring consistent with this codebase (e.g., group usage; DI registrations if required by new services)
- Integration tests added/updated for the endpoint(s)

## Quality Checks

Before finishing, ensure:

- Code compiles with nullable reference types respected
- Endpoint uses `Logger` property (no `ILogger<T>` injection)
- Endpoint is grouped correctly and reachable under `/api/...`
- Validator exists and is exercised by tests
- Integration tests pass and assert meaningful behavior (status codes + response shape)
- No new cross-module boundary violations were introduced
