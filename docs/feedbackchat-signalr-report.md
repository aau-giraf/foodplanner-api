# Real-Time Messaging for FeedbackChat via SignalR — Implementation & Testing Report

**Repository:** `aau-giraf/foodplanner-api`
**Issue:** [#230 — Change message database to use SignalR or something of the like](https://github.com/aau-giraf/foodplanner-api/issues/230)
**Branch:** `feature/signalr-realtime-chat`

---

## 1. Summary

The `FeedbackChat` module previously required clients to poll
`GET api/FeedbackChat/getmessage/{chatThreadId}` to see new messages between
a Parent and a Teacher discussing a specific child. This feature adds a
SignalR hub so connected clients receive new messages instantly, while
keeping the existing SQL persistence as the single source of truth.

---

## 2. Files changed

**New files:**
- `FoodplannerServices/Hubs/ChatHub.cs`
- `Test/FeedbackChatTests/Service/ChatServiceTests.cs`
- `docs/SIGNALR_TESTING.md` *(superseded by this report; kept for reference)*

**Modified files:**
- `FoodplannerApi/Program.cs`
- `FoodplannerServices/FeedbackChat/ChatService.cs`
- `FoodplannerServices/FoodplannerServices.csproj`

**Unchanged (confirmed):** `FoodplannerModels/FeedbackChat/IChatService.cs` — the
interface signature for `AddMessageAsync` did not need to change.

---

## 3. Implementation

### 3.1 `ChatHub`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FoodplannerServices.Hubs;

[Authorize(Roles = "Parent, Teacher")]
public class ChatHub : Hub
{
    public async Task JoinThread(int chatThreadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    public async Task LeaveThread(int chatThreadId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }
}
```

Each connection explicitly joins a group named `thread-{chatThreadId}` after
connecting; nothing joins automatically. Groups are server-side bookkeeping
only (no persistence) and a connection is dropped from all groups on
disconnect.

### 3.2 JWT authentication over WebSocket

Browsers/WebSocket clients can't set custom headers on the handshake, so the
token can't ride in the `Authorization` header the way REST calls do.
`Program.cs` extends the *existing* `AddJwtBearer` events with a single
`OnMessageReceived` handler that reads `access_token` from the query string
— but only for requests to `/hubs/chat`:

```csharp
OnMessageReceived = context =>
{
    var accessToken = context.Request.Query["access_token"];

    if (!string.IsNullOrEmpty(accessToken) &&
        context.HttpContext.Request.Path.StartsWithSegments("/hubs/chat"))
    {
        context.Token = accessToken;
    }

    return Task.CompletedTask;
},
```

For every other path (all REST controllers), this branch is skipped and the
framework falls back to normal `Authorization: Bearer ...` header parsing —
**no change to how REST endpoints authenticate.** The existing
`RoleApproved` claim check in `OnTokenValidated` runs identically for both
paths.

SignalR registration and hub mapping:

```csharp
builder.Services.AddSignalR();
// ...
app.MapHub<ChatHub>("/hubs/chat");
```

### 3.3 Broadcasting on message creation

`ChatService` now takes `IHubContext<ChatHub>` via constructor injection.
Inside `AddMessageAsync`, immediately after
`await _chatRepository.InsertAsync(message)` succeeds (still the first and
unconditional side effect — persistence is never rolled back based on the
broadcast outcome):

1. The sender's first name is looked up (same approach `GetMessagesAsync`
   already uses via its SQL join) and attached to the outgoing DTO.
2. The message is broadcast to `Clients.Group($"thread-{message.ChatThreadId}")`
   via the `"ReceiveMessage"` event.
3. The whole broadcast step (name lookup + `SendAsync`) is wrapped in a
   try/catch. On failure, the error is logged and swallowed —
   `AddMessageAsync` still returns `true` as long as the SQL insert
   succeeded. A broadcast failure never turns a successful message creation
   into an HTTP error for the caller.

### 3.4 Tests

`Test/FeedbackChatTests/Service/ChatServiceTests.cs`, following the existing
Moq/xUnit conventions used elsewhere in the project, mocks
`IHubContext<ChatHub>` → `IHubClients` → `IClientProxy` and covers:

- `InsertAsync` is called with the correct `ChatThreadId`, `Content`, and
  `UserId`.
- `SendAsync("ReceiveMessage", ...)` is called on the correct group
  (`thread-{chatThreadId}`).
- The broadcast DTO includes a non-empty `FirstName`.
- If `SendAsync` throws, `AddMessageAsync` still returns `true` and the
  insert is still verified to have happened — a broadcast failure does not
  propagate or change the return value.

**Result:** `dotnet build` — 0 errors (2 pre-existing, unrelated warnings).
`dotnet test` — all tests passing, nothing skipped.

---

## 4. Design decisions & known limitations

- **Why `ChatHub` lives in `FoodplannerServices`, not `FoodplannerApi`:**
  `ChatService` (in `FoodplannerServices`) needs to inject
  `IHubContext<ChatHub>`. `FoodplannerApi` already has a one-directional
  `ProjectReference` to `FoodplannerServices`; putting the hub in
  `FoodplannerApi` would require a circular project reference, which .NET's
  build system rejects. Moving the hub to `FoodplannerServices` (with a new
  `FrameworkReference` to `Microsoft.AspNetCore.App` so `Hub`/
  `IHubContext`/`[Authorize]` are available in that class library) keeps
  everything buildable without changing the intended dependency injection
  pattern.
- **No new database migration** — no schema changes were needed.
- **Existing authorization gap intentionally preserved:** a chat thread is
  not validated against the caller's actual association with the child —
  any approved Parent/Teacher can join/query any `chatThreadId`. This
  matches `FeedbackChatController`'s current REST behavior and was kept
  consistent for the hub rather than silently tightened, to avoid scope
  creep beyond issue #230. Worth a follow-up issue if stricter enforcement
  is wanted.
- **No delivery retry/guarantee** if a client is briefly disconnected when a
  broadcast fires — out of scope for this issue.
- **Frontend (Flutter) integration is not part of this change** — this is a
  backend-only feature, verified manually via the Postman walkthrough below.
- A pre-existing, unused `Microsoft.AspNetCore.SignalR` 1.1.0
  `PackageReference` in `FoodplannerApi.csproj` (predating shared-framework
  SignalR) was left untouched, since removing it wasn't required for this
  change.

---

## 5. Manual testing guide (Postman)

This section walks through verifying, by hand, that `ChatHub` broadcasts new
messages live to everyone connected to a chat thread. It assumes the API is
running locally (adjust `localhost:8080` to your actual `BACKEND_PORT`).

### Step 1 — Get a JWT for a Parent or Teacher user

The hub requires `[Authorize(Roles = "Parent, Teacher")]` **and**
`RoleApproved = true` on the token — the same requirements as the REST
`FeedbackChatController`.

**Quickest path if you don't have a test user yet** (all via Swagger):

1. `GET api/Users/GetBearerTest` → returns an `Admin` token. Useful only to
   authorize the next step — an `Admin` token will **not** pass the hub's
   role check itself.
2. `POST api/Users/Create` with `{"firstName": "...", "lastName": "...",
   "email": "...", "password": "...", "role": "Parent"}` → returns the new
   user's numeric `id`.
3. `PUT api/Admin/updaterole/{id}` with `Authorization: Bearer <admin token>`
   and body `{"role_approved": true}` — without this, the user's JWT will
   carry `RoleApproved = false` and every authenticated call (REST or hub)
   is rejected.
4. `POST api/Users/Login` with `{"email": "...", "password": "..."}` →
   returns the raw JWT for this Parent/Teacher user. **This is the token you
   use for the WebSocket connection below** — no `Bearer ` prefix.

### Step 2 — Open a WebSocket connection in Postman

1. Postman → **New → WebSocket Request**.
2. URL, with the token as a query parameter (the hub only accepts the token
   via query string, since WebSocket clients can't set custom headers on
   the handshake):
   ```
   ws://localhost:8080/hubs/chat?access_token=<token from Step 1>
   ```
3. Click **Connect**. A successful connection means the JWT and role check
   passed. A `403` means the token's role isn't `Parent`/`Teacher` (e.g. you
   used the `Admin` token from Step 1.1 by mistake — go back and use the
   Login token instead). A `401` usually means `RoleApproved` is still
   `false`, or the token is malformed.

   *(Optional: repeat this in a second Postman WebSocket tab with another
   valid token, to simulate two connected clients receiving the same
   broadcast.)*

### Step 3 — Send the SignalR protocol handshake

SignalR requires a handshake message as the **first** message on the
socket, before invoking any hub method — a JSON object naming the protocol,
terminated by the ASCII **Record Separator** character (`0x1E`), which is
invisible and can't just be typed.

The most reliable way to get the exact bytes into Postman's message field —
**run this in a terminal, not in Postman itself** — then switch to Postman
and paste (Ctrl+V) the result into the message box:

```powershell
Set-Clipboard -Value ('{"protocol":"json","version":1}' + [char]0x1e)
```

(macOS/Linux equivalent: `printf '{"protocol":"json","version":1}\x1e' | pbcopy`,
or from any browser console: `copy('{"protocol":"json","version":1}' + String.fromCharCode(0x1e))`.)

Paste into Postman's message field and click **Send**. A successful
handshake returns `{}` (also `0x1e`-terminated) from the server. You may
also see occasional `{"type":6}` messages appear on their own — these are
SignalR keep-alive pings and can be ignored.

### Step 4 — Join a chat thread

Same trick, different payload — run in the terminal:

```powershell
Set-Clipboard -Value ('{"type":1,"target":"JoinThread","arguments":[<chatThreadId>]}' + [char]0x1e)
```

Paste into Postman and **Send**. There's no response expected for this call
(it's a `void`/`Task`-returning hub method) — no news is good news here.

### Step 5 — Trigger a message and watch it arrive live

From Swagger (or a separate Postman HTTP request), with a valid
Parent/Teacher token:

```
POST api/FeedbackChat/AddMessage
Authorization: Bearer <token>
Content-Type: application/json

{ "chatThreadId": <same id as Step 4>, "content": "Test message" }
```

The WebSocket tab from Step 4 should immediately receive:

```json
{"type":1,"target":"ReceiveMessage","arguments":[{"content":"Test message","firstName":"Test","date":"2026-07-22T22:49:28.8458292+02:00","archived":false,"isEdited":false}]}
```

No polling, no reconnect, no manual refresh needed — and `firstName` is
populated, confirming the sender-name fix is working.

### Troubleshooting

| Symptom | Likely cause |
|---|---|
| `403` on WebSocket connect | Token's role isn't `Parent`/`Teacher` (e.g. used the `Admin` token) |
| `401` on WebSocket connect | `RoleApproved` is `false`, or token expired/malformed |
| Handshake sent but connection drops / `"Handshake was canceled"` | The `0x1e` terminator is missing — you likely typed the message directly instead of pasting the clipboard result; re-run the `Set-Clipboard` command and paste again |
| Connected + handshake OK, but no `ReceiveMessage` ever arrives | `JoinThread` wasn't sent, or its `chatThreadId` doesn't match the one used in `AddMessage`; also check server logs — a broadcast failure is logged but silently swallowed, so `AddMessage` would still report success with nothing arriving |
| Connection drops shortly after connecting | JWT expired — get a fresh token via Login and reconnect |

---

## 6. Verified result

End-to-end tested manually on 2026-07-22 via the steps above. Confirmed
received payload:

```json
{"type":1,"target":"ReceiveMessage","arguments":[{"content":"Test message","firstName":"Test","date":"2026-07-22T22:49:28.8458292+02:00","archived":false,"isEdited":false}]}
```

This confirms: the hub authenticates correctly over WebSocket, group
membership via `JoinThread` works, the broadcast fires immediately after
`AddMessage` persists the message, and the sender's name is correctly
populated (the second follow-up fix).
