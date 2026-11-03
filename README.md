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
| `GET /api/books/{id}` | Get one book; 404 if not found |
| `POST /api/books` | Create a book (Open Library fields + `genre`, `price`, `stock`); returns 201 with the new id, 400 on validation errors, 409 if the ISBN or Open Library key already exists |
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

Angular 21 client in `web/BookStore.UI` (standalone components, signals, PrimeNG 21 and Tailwind CSS 4).

### Prerequisites

- Node.js 20.19 or newer and npm.

### Run

Start the API first (it listens on `http://localhost:5104`), then:

```powershell
cd web\BookStore.UI
npm install
npm start
```

The dev server runs on `http://localhost:4200` and proxies `/api` to the API through `proxy.conf.json`,
so the browser only talks to one origin and the API needs no CORS configuration. If the API runs on a
different port, change the `target` in `proxy.conf.json`.

Run the unit tests with `npm test`

### Styling

PrimeNG provides the components (Aura theme, configured in `src/app/app.config.ts`) and Tailwind provides
layout and spacing. They share one CSS layer order, `theme, base, primeng`, declared in `src/styles.css` and in
the PrimeNG theme options

### Architecture

Clean Architecture with dependencies pointing inward: Presentation -> Application -> Core (Domain), and
Infrastructure -> Core. Nothing in `core` may import from Angular, and only `infrastructure` may use `HttpClient`.

| Folder (`src/app`) | Responsibility |
|---|---|
| `core` | Framework-agnostic domain: models (`Book`, `Genre`, `CatalogBook`, `PagedResult`, `GenreCount`, request shapes), `AppError`, and the repository ports (`BookRepository`, `CatalogRepository`) |
| `application` | Use-case services and app state (signals), plus the `BOOK_REPOSITORY` / `CATALOG_REPOSITORY` injection tokens. Depends only on `core` |
| `infrastructure` | HTTP implementations of the ports, DTO mapping and API error normalization. The only layer that knows endpoint URLs |
| `presentation` | Header, Body, Footer and the dialogs. Talks to use cases, never to `HttpClient` |

Design notes:

- **Ports are plain TypeScript interfaces** in `core`.
- **Errors are values the UI understands.** Repositories emit an `AppError` (`validation`, `notFound`, `conflict`,
  `unavailable`, `timeout`, `unknown`) so components never inspect HTTP status codes.
- **Update only edits store-owned fields** (genre, price, stock, description), matching `PUT /api/books/{id}`.

