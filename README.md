### Technologies Used
- SQLite
- ASP.NET (gRPC)
- SignalR (Websocket)
- Entity Framework + SQLite
### Requirements 
1. .NET 8 (SDK + Runtime)

### Projects
- ESPService: .NET Core Backend (grpc service + websocket)
- ESPService.Test (xUnit)
- ESPClient (Client for ESPService)

### Running Project
- Navigate to the root directory of the project and run the following commands
```powershell
dotnet run --project ESPService
```
- Open a new terminal and run the client
```powershell
dotnet run --project ESPClient
```

## Summary
The project `EPSService` is resposible for generating discount codes using first 7-8 characters of uuid v4.
When started the server automatically ensures sql database is created (to make testing easier)
The kestrel server is configured to run on Http1 and Http2 for Websocket and gRPC respectively. ESPService generates codes and dispatches code generated/used via websocket (SignalR)  

### Note:
When prompted, accept local certificate so the client instance can connect to server
