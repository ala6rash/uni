# UniConnect

A university social-learning web application built with **ASP.NET Core MVC** (net8.0).

## Project structure

The repository contains a single application:

```
UniConnect.Web/   ← ASP.NET Core MVC app (net8.0)
```

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (or SQL Server Express / LocalDB on Windows)

## Setup & run

1. **Configure the connection string**

   Open `UniConnect.Web/appsettings.json` and update the `DefaultConnection` value to point to your SQL Server instance:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UniConnectDb;Trusted_Connection=True;"
   }
   ```

2. **Apply database migrations**

   ```bash
   cd UniConnect.Web
   dotnet ef database update
   ```

3. **Run the application**

   ```bash
   cd UniConnect.Web
   dotnet run
   ```

   The app will be available at `https://localhost:5001` (or the port shown in the console).
