# SnacksPOS

SnacksPOS is a minimal mobile-first point of sale system built with ASP.NET Core.

## Requirements
- .NET 8 SDK
- SQLite

## Running
```bash
dotnet run --project SnacksPOS.Web
```
The app seeds demo users (`admin`/`password`, `demo`/`password`). Navigate to `/SignIn` to sign in.

## Testing
```bash
dotnet test
```

## Extending
Use MediatR commands/queries in `SnacksPOS.Application` and EF Core in `SnacksPOS.Infrastructure`.
