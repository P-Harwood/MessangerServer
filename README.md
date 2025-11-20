This is a small chat system I built in C# as a personal project.

It has two parts:
-Desktop app – .NET MAUI client (Discord-style layout, friends list, DMs, etc.)
-Backend – C# WebSocket + HTTP server for auth, conversations and messages

It’s mainly a learning / portfolio project, not something I’d ship as-is.

#What it does
-Register and sign in over HTTP.
-Create conversations (by user id or username).
-Send and receive messages over WebSockets.
-Friends-style view with:
-Tabs for online, all, pending, blocked, add friend.
-Search / “start conversation” box.
-Basic logging using NLog.
-REST helpers wrapped with a simple Result<T> pattern so network errors are handled cleanly.


#Tech used

Client: .NET MAUI, MVVM-style view models, XAML UI.
Networking (client): HttpClient for REST, ClientWebSocket for realtime traffic, JSON via System.Text.Json.

Server: C# backend exposing /ws/... style HTTP endpoints plus a WebSocket endpoint for live messages.
Logging: NLog.
