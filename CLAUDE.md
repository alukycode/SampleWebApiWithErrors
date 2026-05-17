# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build LogsTriageSampleWebApi/LogsTriageSampleWebApi.csproj

# Run (serves on http://localhost:5018, Swagger UI at /swagger)
dotnet run --project LogsTriageSampleWebApi

# Restore dependencies
dotnet restore LogsTriageSampleWebApi/LogsTriageSampleWebApi.csproj
```

There are no automated tests — the `.http` file (`LogsTriageSampleWebApi/LogsTriageSampleWebApi.http`) contains REST request examples for manual testing of all endpoints.

## Architecture

This is an **ASP.NET Core 10 Web API** using minimal APIs (no controllers). Its purpose is to intentionally trigger specific runtime errors for educational/demonstration use.

### Key files

- **`Program.cs`** — entry point; registers middleware, DI services, and maps endpoints via extension methods
- **`Issues/IssueScenarioEndpointExtensions.cs`** — two extension methods: `AddIssueScenarioHandlers()` (registers all handlers as transient) and `MapIssueScenarioEndpoints()` (maps routes under `/api/issues/`)
- **`Issues/IssueScenarioSupport.cs`** — shared types: `IssueDbContext` (EF Core, SQLite), `IssueWidget` entity, `AsyncPitfallService`, DTOs, and the `DeploymentMode` enum
- **`Issues/IssueCatalog.cs`** — static registry of all 7 issue scenarios (IDs, paths, expected failure descriptions)

### Issue scenario pattern

Each of the 7 handlers lives in `Issues/` and follows the same shape:
- Constructor-injected dependencies
- A single `Handle()` / `HandleAsync()` method returning `IResult`
- Registered as transient in `AddIssueScenarioHandlers()`
- Mapped to a route in `MapIssueScenarioEndpoints()` with `WithSummary()` / `WithDescription()` for OpenAPI

The 7 intentional error scenarios:

| Handler | Error triggered |
|---|---|
| `AutoMapperMissingMapIssueHandler` | Missing AutoMapper type map |
| `MissingDiRegistrationIssueHandler` | Unregistered `IUnregisteredDependency` |
| `MissingConfigurationIssueHandler` | Missing required config key |
| `EfMigrationNotAppliedIssueHandler` | EF Core schema mismatch (migration not applied) |
| `NullableUnexpectedlyNullIssueHandler` | Null dereference / unexpected null |
| `EnumParseMismatchIssueHandler` | JSON deserialization with invalid enum string |
| `IncorrectAsyncUsageIssueHandler` | Fire-and-forget async (no await) |

### Adding a new issue scenario

1. Create `Issues/YourIssueHandler.cs` with a sealed class, constructor injection, and a `Handle` method.
2. Register it in `AddIssueScenarioHandlers()` in `IssueScenarioEndpointExtensions.cs`.
3. Map its endpoint in `MapIssueScenarioEndpoints()`.
4. Add an entry to `IssueCatalog.cs`.

### Data layer

SQLite via EF Core. The database file is `LogsTriageSampleWebApi/issue-scenarios-empty.db`. The `IssueDbContext` has a single `IssueWidgets` DbSet. Migrations are intentionally not applied for the `EfMigrationNotApplied` scenario.

### Deployment

GitHub Actions workflow (`.github/workflows/deploy-api.yml`) publishes to Azure App Service on push to `master`, using secrets `AZURE_API_DEPLOY_USER` and `AZURE_API_DEPLOY_PASS`.
