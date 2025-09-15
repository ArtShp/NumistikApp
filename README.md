# Numistik App

A cross-platform (.NET MAUI) client and ASP.NET Core server for managing numismatic (banknote & coin) collections with role-based access control, invite-based user onboarding, image storage, and paginated browsing.

## Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Data Model](#data-model-entities)
- [Roles & Permissions](#roles-permissions-simplified)
- [Client (MAUI) Overview](#client-maui-overview)
- [Server Overview](#server-overview)
- [Configuration](#configuration)
- [Running (Development)](#running-development)
- [Deployment (Summary)](#deployment-summary)
- [Security Model](#security-model)
- [Pagination Strategy](#pagination-strategy)
- [Image Handling](#image-handling)
- [Seeding & Initial Data](#seeding-initial-data)
- [Roadmap](#roadmap)
- [License](#license)

## Overview

Numistik App consists of:
- A .NET 9 ASP.NET Core Web API (Server)
- A .NET 9 MAUI application (App) consuming the API
- Shared DTOs and enums (in `Shared` project)

## Features

- User registration via invite tokens (first user register with owner token or without token at all)
- JWT auth + refresh token rotation
- Role-based global roles (Owner, Admin and User) and per-collection roles (Owner, Admin, Editor, Viewer)
- Create & manage collections
- Add, list, update, and delete collection items
- Lookup catalogs (countries, types, statuses, qualities, special statuses)
- Upload obverse/reverse item images
- Client-side offline image caching
- Cursor-based pagination (`lastSeenId`, `lastSeenName`)
- CSV-based initial data seeding
- OpenAPI + Scalar UI swagger (dev only)

## Architecture

High-level:
- MAUI App: MVVM pattern with dependency-injected services
- REST API: ASP.NET Core controllers, services encapsulate domain logic
- Persistence: PostgreSQL via EF Core
- Static assets: `./static/images/*`
- Security: JWT (Access + Refresh), roles & claims

Call flow:
```
Client (MAUI) View -> ViewModel -> Service -> REST API Server Controller -> Service -> DbContext -> PostgreSQL
```

## Data Model (Entities)

| Entity | Purpose |
|--------|---------|
| User | Auth principal with `Role` + credential hashes |
| InviteToken | One-time invitation with assigned app role |
| Collection | Logical grouping of items |
| UserCollection | Join mapping user to collection with collection role |
| CollectionItem | A collectible entry including metadata + image refs |
| CollectionItemType / Status / Quality / SpecialStatus | Additional data for items |
| Continent / Country | Geographic taxonomy used by items |

Enums (from `Shared.Models.Common`): `UserAppRole`, `CollectionRole`.

## Roles & Permissions (Simplified)

Selected rules (from services):
- Viewing members/items requires membership OR app Admin
- Creating a collection: any authenticated user
- Managing collection membership/roles: collection Admin+ (can assign roles strictly lower than theirs)
- Editing items: collection role >= Editor
- Deleting items: collection role >= Admin
- Updating another member’s role: only if requester’s role > target role and > new role

## Client (MAUI) Overview

Patterns:
- Dependency injection configured in `MauiProgram`
- Navigation via `Shell` with registered routes
- ViewModels implement:
  - Pagination (`MyCollectionsViewModel`, `CollectionItemsViewModel`)
  - Form state (`CreateCollectionItemViewModel`)
  - Role management (`CollectionRolesViewModel`)
  - Auth flows (`LoginViewModel`, `RegisterViewModel`, `AdminViewModel`)
- Services:
  - `RestApiService`: token management, auto re-login, strongly typed request helpers
  - `CollectionService`, `CollectionItemService`, `LoginService`, `LookupService`, `ImageService`
- Image caching: `ImageService.GetLocalPathAsync` resolves server filenames to locally cached paths

## Server Overview

`Program.cs` responsibilities:
- Add controllers + OpenAPI (dev)
- Configure EF Core PostgreSQL (enum mapping)
- Configure Kestrel + optional certificate
- Configure JWT Bearer auth
- Seed data on startup (`DataSeeder.SeedAll`)
- Serve static files from `static/`

Key services:
- `AuthService`: registration, login, re-login, invite tokens
- `CollectionService`: CRUD + membership + role assignment
- `CollectionItemService`: CRUD + image persistence
- Additional data services (`ContinentService`, `CountryService`, etc.): CRUD

Static file hosting:
- Images physically under `static/images`
- Public URLs: `/static/images/{filename}`

## Configuration

appsettings.json / environment variables (keys):
- `ConnectionStrings:Postgres`
- `AppSettings:Issuer`
- `AppSettings:Audience`
- `AppSettings:Token` (JWT signing secret)
- `AppSettings:OwnerInviteToken` (GUID used to bootstrap first Owner)
- `CertPath`, `CertPassword` (optional production SSL settings for HTTPS)
- (Client) `AppSettings.ServerUrl` stored locally (user-adjustable)

Required secrets for production:
- Strong `AppSettings:Token` (>= 32 random bytes)
- Distinct Issuer/Audience
- Secure database credentials

## Running (Development)

Prerequisites:
- .NET 9 SDK
- PostgreSQL DB (local or Docker)
- (Optional) Android/iOS SDKs for platform targets

1. Set environment variables or `appsettings.Development.json`.
2. Create PostgreSQL database & apply migrations (if migrations exist; otherwise ensure schema creation by EF).
3. Run server
4. Note server URL (Kestrel: https://localhost:5000 or https://0.0.0.0:443 with cert)
5. Launch MAUI client
6. On first run (no users in DB):
- Set `AppSettings:OwnerInviteToken` to a known GUID
- Register via client using that token omitted (logic inserts token automatically if it's a first user and matches configured value)

## Deployment (Summary)

See `docs/SERVER_DEPLOYMENT.md` for deeper steps.
- Provide real TLS certificate (e.g., `CertPath=/app/certs/cert.pfx`, `CertPassword=...`)
- Run database migrations on deploy
- Set environment variables
- Harden Kestrel behind reverse proxy (e.g. nginx)
- Serve static `static/` (already configured), i.e. create this folder and ensure R/W permissions
- Implement logs (structured) & monitoring (add later)

## Security Model

- JWT access tokens (short-lived, duration: `Settings.AccessTokenExpiration`)
- Refresh tokens stored hashed (`RefreshTokenHash`) + expiry (`RefreshTokenExpiryTime`)
- Rotation on each re-login
- Password hashes via `PasswordHasher<User>`
- Claims: `Name`, `NameIdentifier`, `Role`
- Custom authorization attributes

## Pagination Strategy

Cursor-based:
- For name-sorted lists (countries, collections in some contexts): supply `lastSeenName`
- For id-sorted lists: supply `lastSeenId`

Default page size: 10.

## Image Handling

Upload:
- Multipart form-data with fields + `ObverseImage` and `ReverseImage`
- Stored on disk under `static/images/{guid}{ext}`

Download:
- Downloads and caches, path mapping performed via `IImageService`

## Seeding & Initial Data

Server provides initial data.

`DataSeeder` reads CSV under `Data/Seeds/*_seed.csv` for:
- Continents
- CollectionItemTypes
- CollectionItemStatuses
- CollectionItemQualities
- CollectionItemSpecialStatuses
- Countries

Sets sequence values after inserts.

## Roadmap

- Add endpoint list
- Centralized logging
- Rate limiting
- Add search & filtering for items
- Add image thumbnail generation
- Localization

## License

[MIT License](LICENSE.txt)
