# Project Starlights

This is a work-in-progress project intended as an online toolset to enhance tabletop role‑playing games. It's initial focus is creating characters for Dungeons & Dragons. 
There is no public-facing website hosted for this project at this time and more details will be shared as development progresses.

If you'd like to see this project grow, please consider giving it a star :star: — thank you!

## Tech Stack

### Backend

- A modular monolith with domain modules (data elements module, character builder module) build on top of a platform layer
- Entity Framework Core 9 + SQL Server
- .NET 9 Web API using FastEndpoints
- .NET Aspire 9.4 for local orchestration and future Azure deployment
- Serilog for logging, OpenTelemetry for tracing/metrics

### Frontend (Experimental)

- React 19 (TypeScript)
- Vite 7 
- Tailwind CSS 4
- Shadcn UI components

## Prerequisites

- Windows, macOS, or Linux with Docker (required for the local SQL Server container via Aspire)
- .NET SDK 9
- Node.js 20+

## Getting Started (local)

The recommended way to run locally is via the .NET Aspire AppHost, which:

- Creates a persistent SQL Server container
  - With a fixed port `61070` in `AppHost.cs` for development purposes
- Runs EF Core migration workers for Characters and Elements
- Starts the backend API and wires service discovery/telemetry
- Launches the React Builder App (Vite dev server) with an external URL
- Initialize the database from `Initialize Database` command on the `backend` resource or hitting the `/api/elements/initialize` endpoint

Run AppHost:

```powershell
dotnet run -p ./src/aspire/Starlights.AppHost
```

## Tests

- Unit tests live under `*.Tests` projects (Platform, Characters, Elements)
- Integration tests under `src/integration/Starlights.Integration.Tests`

Run all tests:

```powershell
dotnet test
```

## Architecture (high level)

- Modular monolith organized by business capability (character builder, data elements)
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
- All API endpoints are prefixed with `/api` (FastEndpoints route prefix)

## Troubleshooting

- Docker not running: start Docker Desktop (or your container runtime) before launching AppHost
- Port `61070` in use: change the port in `AppHost.cs` and re-run AppHost
- Database not initialized: ensure you start via AppHost so migration workers run to completion

## License

This project is being developed in the open under the [MIT License](./LICENSE). 

## Acknowledgements

This project builds on experience developing [Aurora](https://www.aurorabuilder.com) and modern .NET web tooling.
