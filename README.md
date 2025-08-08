# Project Starlights

Online toolset to enhance tabletop role‑playing games (initially Dungeons & Dragons). Built as a modular monolith on .NET.

## Tech stack

- .NET 9 Web API using FastEndpoints
- Modular monolith with domain modules (Characters, Elements)
- Entity Framework Core 9 + SQL Server
- Serilog for logging, OpenTelemetry for tracing/metrics
- .NET Aspire 9.4 for local orchestration (AppHost, service discovery, SQL container)
- Testing: MSTest + FluentAssertions + Moq

## Repository layout

- `src/apps/Starlights.Application` – backend Web API host (FastEndpoints, Scalar UI)
- `src/aspire/Starlights.AppHost` – .NET Aspire AppHost (runs SQL Server and migration workers, then the backend)
- `src/aspire/Starlights.ServiceDefaults` – common Aspire service defaults (health, OTel, discovery)
- `src/modules/characters` – Characters module (Domain, Data, EF, Endpoints, MigrationService, EventProcessing, Tests)
- `src/modules/elements` – Elements module (Domain, Data, EF, Endpoints, MigrationService, Tests)
- `src/platform` – shared platform (hosting abstractions, data, components like FastEndpoints/Serilog) + tests
- `src/integration/Starlights.Integration.Tests` – integration tests across modules and API
- `assets/` – miscellaneous project assets

Solution file: `Starlights.sln`

## Prerequisites

- Windows, macOS, or Linux with Docker (required for the local SQL Server container via Aspire)
- .NET SDK 9 (repo pins to `9.0.302` via `global.json`)

Verify:

```powershell
dotnet --version
docker version
```

## Getting started (local)

The recommended way to run locally is via the .NET Aspire AppHost, which:

- Creates a persistent SQL Server container on port `61070` (localhost,61070)
- Runs EF Core migration workers for Characters and Elements
- Starts the backend API and wires service discovery/telemetry

Run AppHost:

```powershell
dotnet run -p ./src/aspire/Starlights.AppHost
```

What to expect:

- Console output shows the backend service URL (named `backend`) and migration workers
- Database available at `localhost,61070` (SQL authentication). You can inspect it with SSMS/Azure Data Studio
- In Development, the API exposes:
  - OpenAPI JSON: `/openapi/v1.json`
  - Scalar API UI: `/scalar`
  - Health: `/health` and `/alive`

You can also run just the API (skips containerized SQL + migrations):

```powershell
dotnet run -p ./src/apps/Starlights.Application
```

Note: When running the API directly, ensure a SQL Server instance and schema exist, or use the AppHost first to initialize.

## Tests

- Unit tests live under `*.Tests` projects (Platform, Characters, Elements)
- Integration tests under `src/integration/Starlights.Integration.Tests`

Run all tests:

```powershell
dotnet test
```

## Architecture (high level)

- Modular monolith organized by business capability (Characters, Elements)
- Each module contains Domain/Data/EF/Endpoints; modules communicate via interfaces/events
- Platform layer provides hosting, logging, data, and component wiring

## Database

- Local development uses a SQL Server container provisioned by AppHost
- Connection surfaced to the application via Aspire service discovery
- Static host port: `61070` (configurable in `src/aspire/Starlights.AppHost/AppHost.cs`)

## API docs

In Development, the app maps:

- OpenAPI at `/openapi/v1.json`
- Scalar API Reference UI at `/scalar`

## Troubleshooting

- Docker not running: start Docker Desktop (or your container runtime) before launching AppHost
- Port 61070 in use: change the port in `AppHost.cs` and re-run AppHost
- Database not initialized: ensure you start via AppHost so migration workers run to completion

## License

See [`LICENSE`](./LICENSE).

## Acknowledgements

This project builds on experience with [Aurora](https://www.aurorabuilder.com) and modern .NET web tooling.
