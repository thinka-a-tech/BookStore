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

### Configuration

Open Library is the external book catalog (search only). Its settings live in the `OpenLibrary` section of
`api/src/BookStore.API/appsettings.json` and bind to `OpenLibraryOptions`. No API key is needed.

```json
"OpenLibrary": {
  "BaseUrl": "https://openlibrary.org/",
  "CoversBaseUrl": "https://covers.openlibrary.org/",
  "UserAgent": "BookStoreApp/1.0 (contact: dev@example.com)",
  "TimeoutSeconds": 10
}
```

| Setting | Default | Purpose |
|---|---|---|
| `BaseUrl` | `https://openlibrary.org/` | Base address for the search API |
| `CoversBaseUrl` | `https://covers.openlibrary.org/` | Base address used to build `CoverUrl` values |
| `UserAgent` | `BookStoreApp/1.0 (contact: dev@myproject.com)` | Sent with every request |
| `TimeoutSeconds` | `10` | Request timeout; a timeout surfaces as an external service error |

Any of these can be overridden through environment variables (for example `OpenLibrary__TimeoutSeconds=5`)
or `appsettings.Development.json`.

### Hand-written query

Books per genre, written as raw SQL against SQLite (the genre filter is populated from it):

- **Query:** `BookRepository.GetGenreCountsAsync` in
  [`api/src/BookStore.Infrastructure/Persistence/BookRepository.cs`](api/src/BookStore.Infrastructure/Persistence/BookRepository.cs)
  (`SELECT Genre, COUNT(*) ... GROUP BY Genre`).
- **Slice:** `GetGenreCountsQuery` in `api/src/BookStore.Application/Features/Books/GetGenreCounts`.
- **Endpoint:** `GET /api/books/genres`.

### Trade-offs and decisions

**Error handling with FluentResults.** Handlers return `Result` / `Result<T>` instead of throwing for
expected failures (not found, duplicate ISBN, validation errors). A MediatR pipeline behavior
(`MediatrValidationPipelineBehavior`) runs the FluentValidation validators and returns a failed result,
and `ResultExtensions.ToFailureResult` in the API maps failures to HTTP responses (404, 400
`ValidationProblem`, 500). Controllers keep the success response themselves.

- **Pros:** failure paths are explicit in handler signatures, no exceptions for control flow, and one
  place maps failures to HTTP.
- **Cons:** errors are loosely typed (`NotFoundError` plus a `Property` metadata key for validation),
  so the mapping relies on conventions. The behavior requires every handler to return a FluentResults
  type, and it adds a dependency.
- **Alternative considered:** throw exceptions (`NotFoundException`, FluentValidation's
  `ValidationException`) and map them to `ProblemDetails` in a global `IExceptionHandler`. This is less
  code and lets handlers return plain DTOs, but failure paths become implicit and exceptions are used
  for expected outcomes.

### Endpoints

| Method and route | Purpose |
|---|---|
| `GET /api/books` | List books. Query: `page`, `pageSize` (1-100), `sortBy` (`Title`, `Author`, `Genre`, `Price`, `PublishedYear`, `Stock`), `descending`, `genre` |
| `GET /api/books/genres` | Number of books per genre |
| `PUT /api/books/{id}` | Update a book's store-owned fields (`genre`, `price`, `stock`, `description`); returns the updated book, or 404 if not found |
| `DELETE /api/books/{id}` | Delete a book; returns 204, or 404 if not found |
| `GET /api/catalog/search` | Search Open Library. Query: `q` (2-200 chars), `limit` (1-20, default 10). Returns normalized candidates (title, author, ISBN, year, cover URL, Open Library key); nothing is persisted. 503/504 if Open Library is unavailable or times out |

Request examples are in `api/src/BookStore.API/BookStore.API.http`.

### Tests

Run the application and SQLite repository tests from the repository root:

```powershell
dotnet test api\tests\BookStore.UnitTests\BookStore.UnitTests.csproj
```

Tests are organized by layer in folders within a single `BookStore.UnitTests` project. This keeps
the setup simple for the current project size instead of creating a separate test project per layer.

Repository tests use an isolated in-memory SQLite database; they do not modify `bookstore.db`.
`BookRepositoryTests` implements `IDisposable` so xUnit disposes the EF Core context and SQLite
connection after each test.

## Web

Not started yet.
