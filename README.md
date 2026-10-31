# BookStore

Book Store application: .NET 10 REST API and an Angular 21 web client.
Open Library is the catalog; the SQLite database is the store you own.


## API

### Prerequisites

- [.NET 10 SDK]

### Run

```powershell
dotnet run --project api\src\BookStore.API --launch-profile http
```

The API listens on `http://localhost:5104` (see `Properties/launchSettings.json`).

### Architecture

| Project | Responsibility |
|---|---|
| `BookStore.Domain` | Entities, enums, repository abstractions |
| `BookStore.Application` | Use cases organized by feature slice |
| `BookStore.Infrastructure` | EF Core / SQLite persistence, migrations, seeder |
| `BookStore.API` | Composition root and HTTP endpoints |

### Database

SQLite is the store's source of truth. Nothing needs to be created manually: on startup the API
applies EF Core migrations (creating `bookstore.db` if missing) and seeds sample books when the
`Books` table is empty.

- **Location:** the default connection string is `Data Source=bookstore.db`, relative to the working
  directory (`api/src/BookStore.API` when using `dotnet run --project`). Override it with the
  `ConnectionStrings:BookStore` setting.
- **Reset:** stop the API and delete `bookstore.db*`; it is recreated and reseeded on next start.
- **Migrations:** the local `dotnet-ef` tool is `dotnet-tools.json`.

  ```powershell
  dotnet tool restore
  dotnet ef migrations add <Name> --project api\src\BookStore.Infrastructure --startup-project api\src\BookStore.API --output-dir Persistence\Migrations
  ```
## Web

Not started yet.

