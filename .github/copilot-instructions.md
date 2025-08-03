# Copilot Instructions

## Project Overview

- **Backend**: The backend is a .NET 9 Web API written in C#.
- **Testing Frameworks**:
  - For C# unit tests, always use FluentAssertions for assertions (never use Assert). Use Moq for mocking.

## General C# Best Practices

- Prefer immutability for data objects and minimize mutable state.
- Use explicit access modifiers; default to private for fields.
- Favor async/await for I/O-bound operations.
- Use exception handling judiciously; catch only where you can handle or log meaningfully.
- Apply SOLID principles: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion.
- Use dependency injection for all services and abstractions.
- Prefer using records for simple data containers.
- Use nullability annotations and avoid null reference exceptions.
- Write XML or summary comments for public APIs.
- When implementing an interface, use <inheritdoc /> for XML documentation if the interface already has the comment.
- Use string interpolation over concatenation.
- Avoid magic numbers and strings; use constants or enums.
- Use pattern matching and switch expressions for clarity.
- Use collection initializers and LINQ for concise code.
- Dispose IDisposable objects properly (using statement or dependency injection).
- Use logging for diagnostics, not for control flow.

## Modular Monolith Architecture Best Practices

- Organize code into modules by business capability, not technical layer.
- Each module should have its own domain, application, and infrastructure layer.
- Modules communicate via explicit interfaces or events, not direct references.
- Keep module boundaries strict; avoid leaking internal types.
- Use dependency injection to wire modules together.
- Favor internal visibility for module types; expose only what is necessary.
- Use feature folders or namespaces to group related functionality.
- Keep shared kernel (common code) minimal and stable.
- Use domain-driven design concepts where possible (entities, aggregates, value objects).
- Write integration tests for module interactions.

## Aspire (9.3) Best Practices for Local Development & Azure Deployment

- **Local Development**

  - Use Aspire AppHost to orchestrate and run all services locally for end-to-end testing.
  - Define service dependencies and environment variables in the AppHost manifest for consistent local setup.
  - Use Aspire's service discovery for seamless communication between microservices and modules.
  - Leverage Aspire's local resource provisioning (e.g., local SQL Server, Redis, storage emulators) for realistic dev environments.
  - Use Aspire dashboards for monitoring, logs, and troubleshooting during development.
  - Keep environment configuration (appsettings, secrets) in sync between local and cloud setups.
  - Use feature folders and modular manifests to keep service definitions organized.

- **Deployment to Azure**

  - Use Azure Developer CLI (azd) and Aspire integration for streamlined deployment to Azure.
  - Define infrastructure as code (Bicep) for all required Azure resources (App Service, SQL, Storage, etc.).
  - Use Aspire's environment variable and secret management to securely inject config into cloud services.
  - Monitor deployed services using Azure Monitor, Application Insights, and Aspire dashboards.
  - Use Aspire's health checks and diagnostics for proactive troubleshooting in production.
  - Keep local and cloud manifests aligned to minimize drift and deployment surprises.
  - Use modular manifests and service grouping for scalable, maintainable deployments.

- **General Recommendations**
  - Document all Aspire manifests and environment configurations.
  - Regularly update Aspire and Azure CLI tools to latest versions.
  - Use Aspire's extensibility to add custom resources, health checks, and diagnostics as needed.
  - Test full deployment flows locally before pushing to Azure.
  - Use Aspire's integration with CI/CD pipelines for automated deployments and environment provisioning.

## Entity Framework Core & SQL Server Best Practices

- **Query Performance**

  - Design tables with appropriate indexes for frequent queries and lookups.
  - Use efficient LINQ queries; avoid client-side evaluation and N+1 query problems.
  - Profile and review generated SQL; optimize queries that cause slow performance.
  - Prefer projections (e.g., `.Select`) to limit columns and avoid loading unnecessary data.
  - Use `AsNoTracking()` for read-only queries to improve performance.

- **Data Transfer & Roundtrips**

  - Only load data you need; avoid cartesian explosion when including related entities.
  - Minimize network roundtrips by batching operations and using bulk updates/inserts when possible.
  - Use pagination for large result sets.

- **EF Core Runtime Overhead**

  - Disable change tracking for queries where updates are not needed.
  - Reuse compiled queries for frequently executed queries.
  - Use value converters and shadow properties judiciously.

- **Caching Strategies**

  - Cache frequently accessed data outside the database when appropriate (e.g., in-memory, distributed cache).
  - Be mindful of cache invalidation and consistency.

- **Diagnostics & Monitoring**

  - Enable logging and diagnostics to inspect SQL queries and EF Core behavior.
  - Use tools like SQL Server Profiler, Application Insights, or EF Core logging to monitor performance.
  - Benchmark and measure before optimizing; avoid premature optimization.

- **General Usage**
  - Use migrations for schema changes; keep them organized and reviewed.
  - Prefer explicit relationships and navigation properties.
  - Use transactions for multi-step operations to ensure consistency.
  - Handle exceptions and transient faults gracefully (e.g., retry policies).
  - Secure connection strings and sensitive data.

## Backend (C# Web API with FastEndpoints)

- Generate code targeting .NET 9.
- Follow RESTful API principles.
- Use FastEndpoints for endpoint definitions:
  - Use the REPR pattern (Request, Endpoint, Response).
  - Define endpoints in separate classes, grouped by feature/module.
  - Use dependency injection for services and repositories.
  - Prefer constructor injection for dependencies.
  - Use async Task for endpoint handlers.
  - Validate input using built-in validation features. e.g. Create a Validator<T> class for each request type (it will be automatically used by FastEndpoints).
  - Use attribute-free endpoint definitions for clarity.
  - Secure endpoints by default; use AllowAnonymous only when needed.
  - Use policy/role/claim-based authorization.
  - Return standardized error responses.
  - Use API versioning if needed.
  - Enable CORS globally.
  - Use Identity for authentication and authorization (cookies for auth, GitHub for OAuth).
  - At this time, fastendpoints will be tested using integration tests only, not unit tests. Do not create unit tests for FastEndpoints.

## Unit Testing (C#)

- Use MSTest for unit tests:
  - Name test classes and methods clearly (Class_Method_Scenario).
  - Use [TestClass] and [TestMethod] attributes.
  - Use [DataRow] for parameterized tests.
  - Keep tests isolated and independent.
  - Use Arrange-Act-Assert (AAA) pattern.
    **Always add unit tests when creating a new component:**
    Whenever a new component (class, record, or struct) is created, you must also create a corresponding unit test class. The tests should cover:
  - Default values and construction
  - All public methods and properties
  - Both happy path and edge cases
  - Use MSTest, FluentAssertions, and Moq as described in the testing section
- Use Moq for mocking:
  - Use strongly-typed mocks; avoid magic strings.
  - Setup method/property behaviors with .Setup().
  - Use It.IsAny<T>(), It.Is<T>(predicate) for argument matching.
  - Use .Verify() to check method calls and property sets.
  - Use MockBehavior.Strict for critical tests.
  - Use callbacks and sequences for complex scenarios.
  - Reset mocks between tests if needed.
- Use FluentAssertions for assertions:
  - Use Should() extension methods for readable assertions.
  - Chain assertions for clarity.
  - Use AssertionScope for batching multiple assertions.
  - Prefer expressive assertions (e.g., Should().Be(), Should().Contain(), Should().Throw()).
  - Use custom assertions for domain-specific checks.
- When adding or updating tests:
  - Ensure tests cover both happy path and edge cases.
  - Use descriptive test names that explain the scenario.
  - Keep tests fast and focused; avoid unnecessary dependencies.
  - Use [TestInitialize] and [TestCleanup] for setup/teardown logic.
  - Always build the solution and run the tests on the solution and make sure they pass before reporting that the changes are complete.

## Repository Specifics

- Elements Module:
  - When creating new ElementComponents, also create the corresponding IEntityTypeConfiguration class in the Modules.Elements.Data.EntityFramework project.
