### Technologies Used
- SQLite
- ASP.NET (gRPC)
- SignalR (Websocket)
- Entity Framework + SQLite
### Requirements 
1. .NET 8 (SDK + Runtime)

### Projects
- EPSService: .NET Core Backend (grpc service + websocket)
- EPSService.Test (xUnit)
- EPSClient (Client for EPSService)

### Running Project
- Navigate to the root directory of the project and run the following commands
```powershell
dotnet run --project EPSService
```
- Open a new terminal and run the client
```powershell
dotnet run --project EPSClient
```

## Summary
The project `EPSService` is rEPSosible for generating discount codes using first 7-8 characters of uuid v4.
When started the server automatically ensures sql database is created (to make testing easier)
The kestrel server is configured to run on Http1 and Http2 for Websocket and gRPC rEPSectively. EPSService generates codes and dispatches code generated/used via websocket (SignalR)  

### Note:
When prompted, accept local certificate so the client instance can connect to server
