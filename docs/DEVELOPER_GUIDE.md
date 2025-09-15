# Developer Guide

## Tech Stack

- .NET 9
- .NET MAUI (multi-platform client)
- ASP.NET Core Web API
- EF Core + Npgsql
- JWT Authentication
- CSV Seeding
- MVVM pattern

## Dependency Injection (Client)

Configured in `MauiProgram.cs`:
- Singleton service abstractions (`IRestApiService`, domain services)
- Transient view models

Navigation via `Shell` registered routes.

## Dependency Injection (Server)

`Program.cs` registers:
- DbContext (Npgsql)
- Auth & domain services
- Authentication (JWT Bearer)

## Auth Flow (Client)

1. `LoginService.TryLoginAsync` → `RestApiService.Authorize`
2. Stores access token + refresh token and expiry
3. Each request:
   - If endpoint requires auth & token expired → attempt refresh
   - If refresh expired → failure, user must re-login

## Refresh Logic

Guarded by semaphore to prevent parallel refresh storms.

## Pagination Pattern

- Queries include `lastSeenId` or `lastSeenName`
- Services maintain last-seen cursor
- Iterative fetch until empty (with max page failsafe)

## Adding a New Endpoint (Server)

1. Create Controller or use an existing one (e.g. `CollectionsController`)
2. Inject needed service(s)
3. Validate authorization (global `[AuthorizeAllUsers]` + domain checks in services)
4. Return DTOs

## Adding a New Feature (Client)

1. Define DTO(s) in Shared (preferred) or locally
2. Extend `RestApiEndpoints` with your endpoint(s)
3. Add service method calling `IRestApiService`
4. Create ViewModel + View (XAML)
5. Register route in `AppShell` if navigable

## Seeding

`DataSeeder.SeedAll()` invoked at startup:
- Reads CSV once per table (if empty)
- Adjusts sequence via `setval`

Modify or append CSV files for new data tables.

## Configuration Sources

Precedence: `Environment Variables` > `appsettings.{Environment}.json` > `appsettings.json`.

## Security Enhancements (Future)

- Rate limiting
- Audit logs for role changes & deletions
- Strong password policy enforcement
- Refresh token invalidation on logout (currently just client-side clear)
