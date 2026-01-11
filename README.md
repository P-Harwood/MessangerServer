# Messenger

A lightweight chat system split into a .NET MAUI client and a C# WebSocket/HTTP server. The codebase focuses on learning and experimentation, with a Discord-style UI on the client and a minimal REST + WebSocket backend for auth, conversations, and messages.

## Project layout

```
.
├── Messanger-main/            # .NET MAUI client
│   └── messanger/
│       ├── Screens/           # XAML UI pages
│       ├── Scripts/           # HTTP + WebSocket networking
│       └── DataObjects/       # DTOs for HTTP and WebSocket packets
├── ServerProgram/             # .NET 8 backend
│   ├── Scripts/               # REST handlers + hashing helpers
│   ├── ObjectClasses/         # Server-side DTOs
│   └── DataBase.cs            # PostgreSQL access
└── MessangerServer.sln        # Solution entry point
```

## Key features

- **User registration + sign-in** over HTTP using form-encoded payloads.
- **Live messaging** over WebSockets with broadcast support.
- **Conversation creation** by username (server verifies users + creates records).
- **Friends-style UI** with tabs for online/all/pending/blocked/add-friend.
- **Simple result-wrapping pattern** to surface network errors cleanly.

## Architecture overview

- **Client**: .NET MAUI app with MVVM-style view models and XAML screens.
  - HTTP + WebSocket helpers live in `Messanger-main/messanger/Scripts/NetworkCommunications`.
  - Packet DTOs live in `Messanger-main/messanger/DataObjects`.
- **Server**: .NET 8 console app that hosts:
  - `HttpListener` routes under `http://127.0.0.1:5000/ws/`.
  - WebSocket connections for realtime chat and connected-user broadcasts.
  - PostgreSQL access via `Npgsql`.

## API surface (current)

Base URL: `http://127.0.0.1:5000/ws/`

| Method | Route | Purpose | Payload |
| --- | --- | --- | --- |
| `PUT` | `/Register` | Create user account | `application/x-www-form-urlencoded` (`username`, `password`) |
| `POST` | `/SignIn` | Validate login | `application/x-www-form-urlencoded` (`username`, `password`) |
| `GET` | `/AllUsers` | List users | none |
| `PUT` | `/NewConversationByName` | Create conversation | `application/x-www-form-urlencoded` (`F_UserName`, `Lo_UserName`) |

> Notes:
> - `AddFriend` is currently a placeholder route.
> - There is a commented-out `NewConversation` (by user ID) flow in the server.

### WebSocket packets

The server expects JSON packets with a `PacketHeader`.

- **Sign in (WebSocket)**: `{ "PacketHeader": "SignIn", "username": "..." }`
- **Send message**: `{ "PacketHeader": "SendMessage", "Sender": "...", "Content": "..." }`

## Requirements

- **.NET 8 SDK**
- **.NET MAUI workload** for the client (`dotnet workload install maui`)
- **PostgreSQL** (local connection defaults)

Default database connection values are hardcoded in `ServerProgram/DataBase.cs`:

```
Host=localhost;Port=5432;Username=postgres;Password=password;Database=postgres
```

Update those values to match your local setup.

## Running the server

```bash
dotnet run --project ServerProgram/MessangerServer.csproj
```

The server listens on `http://127.0.0.1:5000/ws/` and accepts both HTTP and WebSocket traffic.

## Running the client

```bash
dotnet build Messanger-main/messanger/messanger.csproj
```

Use your preferred MAUI target (Android, iOS, MacCatalyst, or Windows) to run the app.

## Development notes

- REST calls are defined in `Messanger-main/messanger/Scripts/NetworkCommunications/RestfulRequests.cs`.
- WebSocket traffic is handled in `Messanger-main/messanger/Scripts/NetworkCommunications/WebSockTraffic.cs`.
- Server routing lives in `ServerProgram/Program.cs` and handlers in `ServerProgram/Scripts/RestfulMethods.cs`.

## Roadmap ideas

- Token-based auth instead of plain credential checks.
- Real friend request flows (accept/deny).
- Conversation/message history endpoints.
- Improved error handling + logging across client/server.
