# Project Starlights

This is a work-in-progress project intended as an online toolset to enhance tabletop role‑playing games. Its initial focus is creating characters for Dungeons & Dragons in the form of an online version of [Aurora](https://www.aurorabuilder.com), which was my original creation years ago.

There is no public-facing website hosted for this project at this time, and more details will be shared as development progresses.

If you'd like to see this project grow, please consider giving it a star :star: — thank you!

<hr />

_This is a screenshot from the experimental Development UI in this project._

![Demo UI](./assets/images/development-ui.png)

## Running Locally

This project uses .NET Aspire for local orchestration. You can run it using the command line or Visual Studio.

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- Docker Desktop (or compatible container runtime)
- Visual Studio Code or Visual Studio 2026

### Using Aspire CLI (Recommended)

To run the application using the new Aspire CLI (get it at [aspire.dev](https://aspire.dev)), execute the following command in the root directory:

```bash
aspire run
```

This will start the AppHost, which orchestrates:

- **SQL Server**: A persistent container (port `61070`)
- **Migrations**: Automatically applies EF Core migrations
- **Backend API**: The .NET Web API
- **Frontend**: The React/Vite application
- **Dashboard**: The Aspire dashboard for logs and metrics

### Using Visual Studio 2026

1. Open `Starlights.slnx` in Visual Studio 2026.
2. Ensure `Starlights.AppHost` is set as the startup project.
3. Press **F5** to start debugging.

Once running, the Aspire Dashboard will launch automatically. From there, you can access the frontend application, backend API, and Scalar API documentation.

## Running Tests

You can run the automated test suite using the command line or Visual Studio.

### Using CLI

To run all tests, execute the following command in the root directory:

```bash
dotnet test
```

To run tests with code coverage:

```bash
dotnet test --settings .runsettings
```

### Using Visual Studio 2026

1. Open the **Test Explorer** window (**Test** > **Test Explorer**).
2. Click the **Run All Tests** button (or press **Ctrl+R, A**).

## Architecture

The project follows a **Modular Monolith** architecture, organized by business capability:

- **Elements Module**: Manages game data (classes, abilities, features, rules).
- **Characters Module**: Handles character creation and management.
- **Platform Layer**: Provides shared infrastructure (hosting, logging, data, eventing).

Each module is self-contained with its own domain logic, data persistence, and API endpoints.

## Acknowledgements

This project builds on experience developing [Aurora](https://www.aurorabuilder.com).

## License

This project is being developed in the open under the [MIT License](./LICENSE).
