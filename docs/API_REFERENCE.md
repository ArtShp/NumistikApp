# API Reference (Generated from OpenAPI)

Base API Path Prefix: `/api`  
Authentication: Bearer JWT in `Authorization: Bearer <access_token>`

## Auth

| Endpoint | Method | Request DTO | Response DTO | Description |
|----------|--------|-------------|--------------|-------------|
| `/api/Auth/register` | POST | `UserRegistrationDto.Request` | `UserRegistrationDto.Response` | Register (invite token optional only for first user bootstrap) |
| `/api/Auth/login` | POST | `UserLoginDto.Request` | `RefreshTokenDto.Response` | Issue access + refresh tokens |
| `/api/Auth/refresh-token` | POST | `RefreshTokenDto.Request` | `RefreshTokenDto.Response` | Rotate tokens (refresh must be valid & unexpired) |
| `/api/Auth/create-invite-token` | POST | `InviteTokenDto.Request` | `InviteTokenDto.Response` | Create invite token (requires elevated app role) |

## Collections

| Endpoint | Method | Query Params | Request Body | Response | Purpose |
|----------|--------|-------------|--------------|----------|---------|
| `/api/Collection/all` | GET | `lastSeenId` (Guid, optional) | – | `CollectionDto.Response[]` | Page through ALL collections (admin visibility) |
| `/api/Collection/my` | GET | `lastSeenId` (Guid), `lastSeenName` (string) | – | `CollectionDto.Response[]` | Page user’s collections (sorted by name, then id) |
| `/api/Collection/{collectionId}` | GET | – | – | `CollectionDto.Response` | Get a single collection (membership or app admin required) |
| `/api/Collection/create` | POST | – | `CollectionCreationDto.Request` | `CollectionCreationDto.Response` | Create a new collection (user becomes Owner in that collection) |
| `/api/Collection/update` | POST | – | `CollectionUpdateDto.Request` | `bool` | Update name/description (requires collection Admin+) |
| `/api/Collection/role` | POST | – | `CollectionUpdateRoleDto.Request` | `CollectionDto.Response` | Assign or update a member’s collection role (enforces hierarchy) |
| `/api/Collection/{collectionId}/members` | GET | – | – | `CollectionMembersDto.Response` | List collection members and roles |
| `/api/Collection/{collectionId}/assignable-roles` | GET | – | – | `CollectionRole[]` | Roles current user can assign (roles strictly below user’s role) |

## Collection Items

| Endpoint | Method | Query Params | Body | Response | Notes |
|----------|--------|-------------|------|----------|-------|
| `/api/CollectionItem/{collectionId}` | GET | `lastSeenId` (int) | – | `CollectionItemDto.Response[]` | Paginated by integer id ascending |
| `/api/CollectionItem/{collectionId}/{itemId}` | GET | – | – | `CollectionItemDto.Response` | Get single item |
| `/api/CollectionItem/{collectionId}/{itemId}` | DELETE | – | – | `200 OK` | Requires collection Admin+ or app Admin |
| `/api/CollectionItem/create` | POST (multipart) | – | `CollectionItemCreationDto.Request` (+ images) | `CollectionItemCreationDto.Response` | Fields + optional `ObverseImage`, `ReverseImage` |
| `/api/CollectionItem/update` | POST (multipart) | – | `CollectionItemUpdateDto.Request` (+ optional new images) | `bool` | Partial update (only non-null fields applied) |
| `/api/CollectionItem/image/{filename}` | GET | – | – | Binary file | Returns raw image if exists |

## Additional Data

All these endpoints support cursor-based pagination using either `lastSeenId` (integer) or `lastSeenName` (string) as indicated.

| Category | List Endpoint | Single Endpoint | Create Endpoint | Update Endpoint | Cursor Param |
|----------|---------------|-----------------|-----------------|-----------------|--------------|
| Item Types | `/api/CollectionItemType` | `/api/CollectionItemType/{id}` | `/api/CollectionItemType/create` | `/api/CollectionItemType/update` | `lastSeenId` |
| Item Statuses | `/api/CollectionItemStatus` | `/api/CollectionItemStatus/{id}` | `/api/CollectionItemStatus/create` | `/api/CollectionItemStatus/update` | `lastSeenId` |
| Item Qualities | `/api/CollectionItemQuality` | `/api/CollectionItemQuality/{id}` | `/api/CollectionItemQuality/create` | `/api/CollectionItemQuality/update` | `lastSeenId` |
| Item Special Statuses | `/api/CollectionItemSpecialStatus` | `/api/CollectionItemSpecialStatus/{id}` | `/api/CollectionItemSpecialStatus/create` | `/api/CollectionItemSpecialStatus/update` | `lastSeenId` |
| Continents | `/api/Continent` | `/api/Continent/{id}` | `/api/Continent/create` | `/api/Continent/update` | `lastSeenId` |
| Countries | `/api/Country` | `/api/Country/{id}` | `/api/Country/create` | `/api/Country/update` | `lastSeenName` |

## Common Query Parameters

| Name | Type | Used In | Purpose |
|------|------|---------|---------|
| `lastSeenId` | Guid (collections) or int (items/lookups) | Pagination | Cursor for next page (exclusive) |
| `lastSeenName` | string | Countries, user collections | Secondary cursor for stable ordering (name + id) |

---

## Rate Limits

Not implemented yet.
