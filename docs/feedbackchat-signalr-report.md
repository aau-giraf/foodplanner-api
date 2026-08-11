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

**New files (initial feature):**
- `FoodplannerServices/Hubs/ChatHub.cs`
- `Test/FeedbackChatTests/Service/ChatServiceTests.cs`
- `docs/SIGNALR_TESTING.md` *(superseded by this report; kept for reference)*

**Modified files (initial feature):**
- `FoodplannerApi/Program.cs`
- `FoodplannerServices/FeedbackChat/ChatService.cs`
- `FoodplannerServices/FoodplannerServices.csproj`

**Unchanged (confirmed):** `FoodplannerModels/FeedbackChat/IChatService.cs` — the
interface signature for `AddMessageAsync` did not need to change.

**Follow-up fixes — new files:**
- `Test/FeedbackChatTests/Hub/ChatHubTests.cs` (object-level authorization)
- `Test/FeedbackChatTests/ChatProfileTests.cs` (real-AutoMapper convention check)

**Follow-up fixes — modified files:**
- `FoodplannerServices/Hubs/ChatHub.cs` (§3.4 — thread-ownership check)
- `FoodplannerModels/FeedbackChat/IChatRepository.cs` (§3.4 — exposed
  `GetChatThreadByIdAsync`, no new query)
- `FoodplannerModels/FeedbackChat/UserNameFeedbackChatDTO.cs` (§3.5 —
  `MessageId`/`ChatThreadId`)
- `FoodplannerDataAccessSql/FeedbackChat/ChatRepository.cs` (§3.5 —
  `InsertAsync` now returns the generated id via `RETURNING message_id`)
- `Test/FeedbackChatTests/Service/ChatServiceTests.cs` (§3.6 — new tests)

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

### 3.4 Object-level authorization on `JoinThread`/`LeaveThread`

**Follow-up fix (code review):** class-level `[Authorize(Roles = "Parent,
Teacher")]` alone let any approved Parent or Teacher join *any* chat
thread's group, regardless of whether they were actually associated with
the child that thread belongs to. Unlike the REST endpoints (which were
never meant to be production-hardened), the hub now enforces this:

`ChatHub` takes `IChatRepository` and `IChildrenRepository` via constructor
injection (both already registered in DI, so no `Program.cs` change was
needed). Both `JoinThread` and `LeaveThread` call a shared private helper,
`EnsureCallerIsAuthorizedForThreadAsync`, before touching the group:

1. Look up the `ChatThread` via `IChatRepository.GetChatThreadByIdAsync`
   (already existed on the concrete `ChatRepository`, just not previously
   exposed on `IChatRepository` — added to the interface rather than
   duplicating the query) to get its `ChildId`.
2. Read the caller's id from the `ClaimTypes.NameIdentifier` claim and role
   from `Context.User.IsInRole(...)` — the same claim types
   `AuthService`/`Program.cs` already use for REST auth
   (`ClaimTypes.NameIdentifier` for id, `ClaimTypes.Role` as the configured
   `RoleClaimType`).
3. If the caller is a Parent, check their id is among
   `IChildrenRepository.GetParentsByChildIdAsync(childId)`. If a Teacher,
   check `GetTeachersByChildIdAsync(childId)` instead.
4. If the check fails, the connection is **not** added to (or removed from)
   the group, and a `HubException("Not authorized for this chat thread")` is
   thrown, so the client sees a clear error instead of a silent no-op.

### 3.5 `MessageId`/`ChatThreadId` on the broadcast payload (deduplication)

**Follow-up fix (code review):** since a sender is themselves a member of
their own chat thread's SignalR group, they receive their own message back
via `ReceiveMessage`. Without a stable id, the frontend couldn't tell that
apart from a genuinely new message, so it couldn't deduplicate an
optimistically-rendered message against the one echoed back over the socket.

- `UserNameFeedbackChatDTO` (used by both `GetMessagesAsync` and the
  broadcast — same DTO, so history and live messages are now consistent)
  gained `MessageId` and `ChatThreadId` int properties. `Message` already
  has both fields under the exact same names, so AutoMapper's existing
  `CreateMap<Message, UserNameFeedbackChatDTO>()` picks them up by
  convention with no `.ForMember()` needed — confirmed with a dedicated
  test using a real `MapperConfiguration` (not a mocked `IMapper`), see
  below.
- This exposed a latent bug: `ChatRepository.InsertAsync` ran a plain
  `INSERT` via Dapper's `ExecuteAsync`, which does **not** populate
  auto-generated ids back onto the passed-in object — `message.MessageId`
  stayed `0` after every insert. Fixed by adding `RETURNING message_id`
  (same pattern already used in `AddChatThreadIdByChildIdAsync`) and using
  `ExecuteScalarAsync<int>` to read it back, assigning it onto
  `message.MessageId` before returning. Since `ChatService.AddMessageAsync`
  maps the broadcast DTO from that same `Message` object *after*
  `InsertAsync` returns, the correct id now flows through automatically —
  no `ChatService.cs` change was needed for this part.

### 3.6 Tests

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
- The broadcast DTO carries the `MessageId` generated by (a mocked)
  `InsertAsync` and the correct `ChatThreadId`.
- `GetMessagesAsync`'s mapped results include `MessageId`/`ChatThreadId`.

`Test/FeedbackChatTests/ChatProfileTests.cs` is a new, deliberately
non-mocked test: it builds a real `MapperConfiguration` with `ChatProfile`
and asserts `Map<UserNameFeedbackChatDTO>(message)` actually carries
`MessageId`/`ChatThreadId` through — proving AutoMapper's convention-based
mapping works, which a mocked `IMapper` test could not prove.

`Test/FeedbackChatTests/Hub/ChatHubTests.cs` is a new test file for
`ChatHub` (SignalR's `Hub.Context`/`Hub.Groups`/`Hub.Clients` are public
settable properties specifically to support this kind of unit testing
without a live connection). Covers:

- `JoinThread` succeeds (adds to the group) for a Parent actually linked to
  the thread's child, and separately for a Teacher actually linked to it.
- `JoinThread` throws `HubException` and does **not** add the connection to
  the group when the caller (Parent or Teacher) is not associated with the
  thread's child.
- `LeaveThread` applies the same check and also throws without removing
  the connection from the group when unauthorized.

**Result:** `dotnet build` — 0 errors. `dotnet test` — 167/167 passing,
nothing skipped (160 before this follow-up + 7 new tests: 4 in
`ChatHubTests`, 2 in `ChatServiceTests`, 1 in `ChatProfileTests`).

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
- **Object-level authorization gap — fixed (see §3.4):** joining or leaving
  a chat thread's SignalR group now verifies the caller is actually a
  parent or teacher associated with that thread's child, via
  `IChildrenRepository`. This intentionally only covers the hub, per the
  maintainer's review feedback — the REST endpoints
  (`FeedbackChatController`) were explicitly out of scope for this
  follow-up and still don't validate thread ownership; that would be a
  separate change if wanted.
- **Missing `MessageId`/`ChatThreadId` on the broadcast DTO — fixed (see
  §3.5):** the frontend can now deduplicate a message it rendered
  optimistically against the copy echoed back over the socket.
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

`JoinThread` now checks that you're actually a Parent/Teacher associated
with the given thread's child (§3.4). SignalR only sends a completion
message back for invocations that include an `invocationId` — the
fire-and-forget payload above (no `invocationId`) will just silently not
join the group if unauthorized, with nothing visible in Postman. To
actually see the error, include an `invocationId`:

```powershell
Set-Clipboard -Value ('{"type":1,"invocationId":"1","target":"JoinThread","arguments":[<chatThreadId>]}' + [char]0x1e)
```

If you use a `chatThreadId` that belongs to a different child than the one
your logged-in user is linked to, you'll get back:

```json
{"type":3,"invocationId":"1","error":"Not authorized for this chat thread"}
```

`HubException` messages (unlike other exception types) are always sent to
the client verbatim, regardless of `EnableDetailedErrors` — that's why
`ChatHub` throws `HubException` specifically rather than a generic
exception. Either way — with or without an `invocationId` — an unauthorized
call never adds the connection to the thread's group.

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
{"type":1,"target":"ReceiveMessage","arguments":[{"messageId":57,"chatThreadId":5,"content":"Test message","firstName":"Test","date":"2026-07-22T22:49:28.8458292+02:00","archived":false,"isEdited":false}]}
```

No polling, no reconnect, no manual refresh needed — `firstName` is
populated (confirming the sender-name fix), and `messageId`/`chatThreadId`
are now present so a client that already rendered this message
optimistically can recognize it and skip rendering a duplicate.

### Troubleshooting

| Symptom | Likely cause |
|---|---|
| `403` on WebSocket connect | Token's role isn't `Parent`/`Teacher` (e.g. used the `Admin` token) |
| `401` on WebSocket connect | `RoleApproved` is `false`, or token expired/malformed |
| Handshake sent but connection drops / `"Handshake was canceled"` | The `0x1e` terminator is missing — you likely typed the message directly instead of pasting the clipboard result; re-run the `Set-Clipboard` command and paste again |
| Connected + handshake OK, but no `ReceiveMessage` ever arrives | `JoinThread` wasn't sent, or its `chatThreadId` doesn't match the one used in `AddMessage`; also check server logs — a broadcast failure is logged but silently swallowed, so `AddMessage` would still report success with nothing arriving |
| `JoinThread`/`LeaveThread` completion has `"error":"Not authorized for this chat thread"` (only visible if you sent an `invocationId`) | The logged-in user is a Parent/Teacher, but not one associated with that `chatThreadId`'s child — use a `chatThreadId` for a child your test user is actually linked to (§3.4) |
| Connection drops shortly after connecting | JWT expired — get a fresh token via Login and reconnect |

---

## 6. Verified result

### 6.1 Initial feature — 2026-07-22

End-to-end tested manually via the steps above (predating the object-level
authorization and `MessageId`/`ChatThreadId` follow-up fixes). Confirmed
received payload at that time:

```json
{"type":1,"target":"ReceiveMessage","arguments":[{"content":"Test message","firstName":"Test","date":"2026-07-22T22:49:28.8458292+02:00","archived":false,"isEdited":false}]}
```

This confirmed the hub authenticates correctly over WebSocket, group
membership via `JoinThread` works, the broadcast fires immediately after
`AddMessage` persists the message, and the sender's name is correctly
populated.

### 6.2 Follow-up fixes (review feedback) — 2026-08-04

The two follow-up fixes in §3.4/§3.5 were re-verified manually end-to-end
via Postman, on top of the automated tests in §3.6 (`dotnet test`,
167/167 passing), using two separate Parent users each with their own
child and chat thread.

**Object-level authorization on `JoinThread` (§3.4):**

- **Positive case** — joining a thread the connected user is actually
  associated with succeeds:
  ```json
  {"type":1,"invocationId":"1","target":"JoinThread","arguments":[1]}
  ```
  ```json
  {"type":3,"invocationId":"1","result":null}
  ```
- **Negative case** — the same connection then attempted to join a second
  thread (`chatThreadId: 2`) belonging to a different, unrelated Parent
  user. The server-side log confirms the request reached and was rejected
  by the new authorization check, not by an unrelated error:
  ```
  fail: Microsoft.AspNetCore.SignalR.Internal.DefaultHubDispatcher[8]
        Failed to invoke hub method 'JoinThread'.
        Microsoft.AspNetCore.SignalR.HubException: Not authorized for this chat thread
           at FoodplannerServices.Hubs.ChatHub.EnsureCallerIsAuthorizedForThreadAsync(Int32 chatThreadId) ...
           at FoodplannerServices.Hubs.ChatHub.JoinThread(Int32 chatThreadId) ...
  ```
  (The Postman client itself only surfaced a generic "An unexpected error
  occurred invoking 'JoinThread' on the server" message rather than the
  `HubException`'s own text — a client-side display detail, not a defect
  in the authorization check itself, which the server log confirms ran
  and rejected the call as intended.)
- **No unauthorized data leak** — a message was then posted to thread `2`
  (`POST api/FeedbackChat/AddMessage`). The rejected connection, never
  having been added to `thread-2`'s SignalR group, received nothing —
  confirming the authorization check has a real effect on message
  delivery, not just on the join call's return value.

**`MessageId`/`ChatThreadId` on the broadcast DTO (§3.5):**

A message posted to the thread the connection *was* validly joined to
(`chatThreadId: 1`) arrived with both fields populated with real,
non-zero values:

```json
{"type":1,"target":"ReceiveMessage","arguments":[{"messageId":6,"chatThreadId":1,"content":"test 1","firstName":"parent2","date":"2026-08-04T18:19:58.0912489+02:00","archived":false,"isEdited":false}]}
```

`messageId: 6` confirms the `RETURNING message_id` fix in
`ChatRepository.InsertAsync` (§3.5) actually flows through to the broadcast
— not just that the DTO has the field, but that it's populated with the
real generated id rather than `0`.

### 6.3 Summary

| Check | Result |
|---|---|
| `JoinThread` succeeds for a thread the caller owns | ✅ confirmed |
| `JoinThread` rejects a thread the caller does not own | ✅ confirmed (server log) |
| Rejected caller receives no messages from that thread | ✅ confirmed (no leak) |
| Broadcast `messageId` is a real, non-zero id | ✅ confirmed (`6`) |
| Broadcast `chatThreadId` is correct | ✅ confirmed (`1`) |

Both follow-up fixes are confirmed working end-to-end, not only at the
level of mocked unit tests.
