# User Guide

## Getting Started

1. Install and launch the NumistikApp client
2. Configure the Server URL (Login screen > settings icon) if different from default
3. If you are the first user (bootstrap scenario), ensure the server is configured with `AppSettings:OwnerInviteToken`. Register normally (token auto-applied)
4. Otherwise, obtain an invite token from an Admin and register using it

## Authentication

- Login with username/password
- Sessions auto-refresh while refresh token valid
- Use the Logout option to clear all tokens

## Collections

- View “My Collections” tab to see collections you belong to
- Create a collection (enter Name + optional Description)
- Open a collection to see items
- Open Roles to manage members (if you have sufficient role)

## Roles (per collection)

- Viewer: Read-only
- Editor: Add/edit items
- Admin: Manage roles (below theirs), delete items
- Owner: Assigns admins, highest role

You cannot:
- Change your own role
- Assign a role equal/higher than your own
- Modify members if you are below Admin

## Items

- In a collection, press a button to load more.
- Add Item:
  - Required: Type, Country, Status, Value, Currency
  - Optional: Special Status, Quality, Images, Additional Info, Description
- Edit Item: Only if permissions allow (Editor+)
- Delete Item: Only if permissions allow (Admin+)

## Images

- Pick images from device (obverse/reverse). Large images may increase upload time
- Images appear after upload, caching is automatic

## Additional Data

Types, Countries, Statuses, Qualities, Special Statuses are loaded on first form open and cached.

## Admin Area

Accessible if your global app role is Admin or higher:
- Generate invite tokens
- Copy newly generated token for distribution

## Troubleshooting

| Issue | Resolution |
|-------|------------|
| Cannot login | Verify credentials and server URL |
| “Registration failed” | Ensure valid invite token or bootstrap condition |
| No roles to assign | You may not be Collection Admin+ |
| Item creation fails | Ensure required fields filled and you have Editor+ role |
| Images not visible | Check network / server `/static/images` availability |

## Privacy & Security

- Passwords hashed (never stored plain)
- Tokens stored locally, logout clears them
- Do not share invite tokens publicly
