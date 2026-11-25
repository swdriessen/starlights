# Project Starlights

This is a work-in-progress project intended as an online toolset to enhance tabletop role‑playing games. Its initial focus is creating characters for Dungeons & Dragons in the form of an online version of [Aurora](https://www.aurorabuilder.com), which was my original creation years ago.

If you'd like to see this project grow, please consider giving it a star :star: — thank you!

There is no public-facing website hosted for this project at this time, and more details will be shared as development progresses.

<hr />

_A screenshot from the experimental Development UI in this project._

![Demo UI](./assets/images/development-ui.png)

## Running Locally

This project uses .NET Aspire for local orchestration. You can run it using Visual Studio or the command line.

### Prerequisites

- .NET 11 SDK (version selected by `global.json`)
- Aspire CLI 17.0.0 or later, matching the Aspire 17 AppHost and hosting integrations
- Node.js 20.19+
- Docker Desktop (or compatible container runtime)
- Visual Studio (recent version) or Visual Studio Code

Deployment machines must meet .NET 11's CPU requirements: x86-64-v2 on x64, or LSE support on Windows Arm64.

The AppHost currently retains `Aspire.AppHost.Sdk` and `AddProject` for its existing launch/debug workflow. Aspire 17 supports these APIs with deprecation warnings. Migration to `Microsoft.NET.Sdk` and `AddDotnetProject` is a separate step because it changes project launching and coordinated builds, and the replacement does not yet support watch; see the [Aspire migration guidance](https://aspire.dev/integrations/dotnet/migrate-dotnet-project-resource/).

### Using Visual Studio

1. Open `Starlights.slnx` in Visual Studio.
2. Ensure `Starlights.AppHost` is set as the startup project with `https` as the launch profile.
3. Press **F5** to start debugging.

Once running, the Aspire Dashboard will launch automatically. From there, you can access the frontend application, backend API, and Scalar API documentation.

### Using Aspire CLI

To run the application using the Aspire CLI (see [aspire.dev](https://aspire.dev)), execute the following command in the root directory:

```bash
aspire run
```

This will start the AppHost, which orchestrates:

- **SQL Server**: A container (port `61070`)
- **Migrations**: Automatically applies EF Core migrations
- **Backend API**: The .NET Web API
- **Frontend**: The React/Vite application
- **Dashboard**: The Aspire dashboard for logs and metrics

### Initial Setup

Aspire applies the Identity, FileManagement, Elements and Characters migrations before starting the API. Provision an account before opening the builder; there is no public registration or account-administration API.

The Characters migration host explicitly runs without an authenticated user. It can construct the ownership-aware persistence services for schema migration without impersonating an account or bypassing ownership checks for Character operations.

From a private operator terminal, supply the SQL Server connection string for the same database used by Aspire through `ConnectionStrings__starlights-db` (or an operator-managed configuration provider). Do not commit credentials or pass passwords as command arguments:

```powershell
${env:ConnectionStrings__starlights-db} = "<operator-connection-string>"
dotnet run --project src\apps\Starlights.Application --no-launch-profile -- accounts create <username>
dotnet run --project src\apps\Starlights.Application --no-launch-profile -- accounts reset-password <username>
dotnet run --project src\apps\Starlights.Application --no-launch-profile -- accounts disable <username>
dotnet run --project src\apps\Starlights.Application --no-launch-profile -- accounts enable <username>
```

Create and reset prompt for the password twice without echoing it; redirected password input is rejected. Passwords must satisfy ASP.NET Core Identity's default password policy. Accounts require a unique username, but no email or profile.

Sign in through the builder. Sessions have an eight-hour inactivity timeout and no remember-me option. Application restarts preserve valid sessions; sign-out revokes only the current session, while password reset and disabling revoke every account session by the next API request. Re-enabling requires a new sign-in. Keep the database, its persisted authentication keys and its backups private to the application and authorized operators.

The retained Character routes now require sign-in and enforce owner isolation, including lists and nested selections. The ownership migration leaves pre-existing ownerless development Characters inaccessible rather than assigning them to an arbitrary account. Reset disposable development data if needed; new Characters receive the authenticated user's stable GUID. This is a security adapter for the retained model, not the target Character redesign.

To initialize sample content, use the authenticated `POST /api/elements/initialize` request in [Identity.http](src/apps/Starlights.Application/Identity.http) after signing in and obtaining a fresh antiforgery token. Initialization is no longer a state-changing GET or an anonymous Aspire dashboard action.

### Frontend Dependencies

Install frontend dependencies from `src/frontend` using `npm ci`; the root lockfile covers all npm workspaces. Run `npm run build` there to build the shared packages and applications.

The frontend uses TypeScript 7 and Oxlint. Run `npm run lint` from `src/frontend` to lint every application and shared package, or use `npm run lint -w <workspace>` for one workspace. All workspaces use the shared `src/frontend/.oxlintrc.json` configuration.

Oxlint's native rules cover JavaScript/TypeScript checks, Rules of Hooks, Hook dependencies, and Fast Refresh exports. React Compiler-specific diagnostics from eslint-plugin-react-hooks are not included; this setup does not use ESLint or JavaScript lint plugins.

Browser API calls use relative `/api` paths, same-origin cookies and fresh antiforgery tokens for writes. Vite resolves its backend proxy from `VITE_API_BASE` or Aspire service discovery; the standalone fallback is `http://localhost:5240`. Production hosting must expose UI and API under one HTTPS origin. Only loopback development may use plain HTTP.

With the development API running, regenerate the typed client contracts from `src/frontend`:

```powershell
npm run generate -w @starlights/api-client -- http://localhost:5240/openapi/v1.json
```

Use the API's actual HTTP address when Aspire assigns a different port. Generation covers Identity, FileManagement and retained legacy routes; the typed client is used by authentication and the private file library, while retained consumers use the secured legacy adapter. The pinned generator runs with its own TypeScript 5 peer via `npm exec`, isolated from the application's TypeScript 7. Commit regenerated contracts alongside endpoint changes. [Identity.http](src/apps/Starlights.Application/Identity.http) demonstrates token, sign-in, session and sign-out requests.

### Private File Library

Open **Private files** in the signed-in account bar, or navigate to `/files`. Upload, list, view and permanently delete your own PNG, JPEG and WebP images. Images must be at most 5 MiB and 4096 pixels per side; animated images, SVG and malformed content are rejected. Originals, including embedded metadata, are preserved rather than sanitized or re-encoded. Uploading again creates a new file identifier.

`Modules.FileManagement` owns SQL metadata in `file_management` and private blob content. Aspire supplies the `files` storage connection, starts a persistent Azurite emulator and waits for the FileManagement migration worker. The emulator skips API-version-header checks because the current Azure SDK is newer than its supported version list; this setting applies only to local emulation. The worker creates the `private-files` container without public access and fails startup if an existing container is public.

Outside Aspire, supply both `ConnectionStrings__starlights-db` and `ConnectionStrings__files` privately to the FileManagement migration worker and API. Run `Modules.FileManagement.MigrationService` before starting the API. Keep storage credentials and service endpoints out of frontend configuration and source control. Browsers receive only authenticated same-origin `/api/files/{id}/content` addresses, never blob URLs or SAS tokens.

The upload transport is JSON with `fileName` and base64 `contentBase64`. The decoded limit remains 5 MiB; the HTTP body limit is 8 MiB to accommodate base64 and JSON overhead. Lists are owner-filtered, stably ordered and bounded to 50 entries per page. [Files.http](src/apps/Starlights.Application/Files.http) demonstrates the Minimal API requests.

Deletion first records its intent in SQL, then removes the blob and metadata. An interrupted deletion remains visible as pending, cannot serve content and can be retried after reconnecting or restarting. Failed upload metadata writes trigger blob cleanup; if cleanup also fails, both errors and the opaque file identifier are logged for operator recovery. Check these explicit errors rather than assuming a failed request completed.

This is a standalone private image library. Character assignment and protection against deleting referenced files are deferred together; do not inject these identifiers into the retained portrait-URL field. Quotas, sharing and additional file formats remain future work.

Identity and private storage are F00 increments, not completion of the platform foundation. See the [roadmap](docs/ROADMAP.md#f00-establish-the-platform-foundation) for verification status and remaining local/private-network checks. The target Character-builder journey is still later work.

## Running Tests

You can run the automated test suite using Visual Studio or the command line.

### Using Visual Studio 2026

1. Open the **Test Explorer** window (**Test** > **Test Explorer**).
2. Click the **Run All Tests** button (or press **Ctrl+R, A**).

### Using CLI

To run all tests, execute the following command in the root directory:

```bash
dotnet test
```

## Architecture

The project follows a **Modular Monolith** architecture, organized by business capability:

- **Elements Module**: Manages game data (classes, abilities, features, rules).
- **Characters Module**: Handles character creation and management.
- **Identity Module**: Owns local accounts, credentials, durable cookie sessions and shared authenticated-user integration.
- **FileManagement Module**: Owns private file identifiers, owner-filtered metadata and immutable blob content; consumers own the meaning of file references.
- **Platform Layer**: Provides shared infrastructure (hosting, logging, data, eventing).

Each module is self-contained with its own domain logic, data persistence, and API endpoints.

## Acknowledgements

This project builds on my experience developing [Aurora](https://www.aurorabuilder.com), a character builder for Windows.

## License

This project is being developed in the open under the [MIT License](./LICENSE).
