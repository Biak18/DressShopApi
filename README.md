# DressShop.Api

DressShop backend — ASP.NET Core (.NET 10) following the Clean Architecture +
Vertical Slice conventions in `ARCHITECTURE.md`.

## Stack

- **.NET 10** (`global.json` pins `10.0.100`, `rollForward: latestFeature`)
- **PostgreSQL only** via `Npgsql.EntityFrameworkCore.PostgreSQL` (Supabase-hosted).
  There is no SQL Server provider in this project.
- **Supabase Auth** — JWT Bearer (`Supabase:Url` + `Supabase:AnonKey`), `Admin` policy
  for back-office writes.
- **MediatR** dispatch (`ISender.Send(...)`), FluentValidation pipeline,
  centralized `GlobalExceptionHandler` → `ProblemDetails`.
- **OpenAPI + Scalar** (`/openapi/v1.json`, `/scalar`). No `/health` endpoint by design.

## Layout

```
DressShop/
├── DressShop.sln
├── global.json                    # pins the SDK version
├── Directory.Build.props          # nullable, warnings-as-errors, central package mgmt
├── Directory.Packages.props       # central NuGet versions (EF 10.0.11 + Npgsql)
├── src/
│   ├── DressShop.Domain/          # no dependencies, entities + domain exceptions
│   ├── DressShop.Application/     # -> Domain. Features/* (commands, queries, handlers, validators, DTOs)
│   ├── DressShop.Infrastructure/  # -> Application, Domain. AppDbContext, configs, Supabase/Gemini clients
│   └── DressShop.Api/             # -> Application, Infrastructure. Controllers, auth, middleware
└── tests/
    ├── DressShop.UnitTests/       # -> Domain, Application
    └── DressShop.IntegrationTests/ # -> Api (WebApplicationFactory boot test)
```

## Features

Public reads, authenticated writes, admin-only back office:

| Area | Endpoints |
|---|---|
| Products | `GET /api/products`, `GET /api/products/{id}`, `GET /api/products/slug/{slug}` (public); `POST/PUT/DELETE` (Admin) |
| Categories | `GET /api/categories` (public); `POST` (Admin). Full CRUD at `GET/POST/PUT/DELETE /api/admin/categories` (Admin) |
| Cart | `GET/POST/PUT/DELETE /api/cart*` (authenticated) |
| Orders | `GET/POST /api/orders` (authenticated); `PATCH /api/admin/orders/{id}/status`, `GET /api/admin/orders` (Admin) |
| Favorites | `GET /api/favorites*`, wishlist + status/ids (authenticated) |
| Reviews | `GET /api/products/{id}/reviews`, summary + eligibility; `POST/PUT/DELETE` (authenticated, owner) |
| Loyalty | `GET /api/loyalty/*`, redeem 200/400/800-point tiers (authenticated) |
| Notifications | `GET /api/notifications*`, mark read, preferences (authenticated) |
| Addresses | `GET/POST/PUT/DELETE /api/addresses*` + set-default (authenticated) |
| Profiles | `GET/PUT /api/profiles/me` (authenticated) |
| Auth | `POST /api/auth/register|login|refresh|logout|forgot-password` (rate-limited `auth` policy) |
| Admin | `GET /api/admin/stats|low-stock`, `PATCH /api/admin/products/{id}/active|variants/{id}/stock` (Admin) |
| Assistant | `POST /api/assistant/chat` (Gemini, server-side key) |

## Configuration — user-secrets (nothing to add manually to appsettings)

`appsettings.json` ships with empty `ConnectionStrings:DefaultConnection` on purpose.
Tables already exist in Supabase, so **no EF migrations are needed or run**.
Required secrets live in user-secrets / environment:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..." --project src/DressShop.Api
dotnet user-secrets set "Supabase:AnonKey" "<supabase-anon-key>" --project src/DressShop.Api
# Optional: dotnet user-secrets set "Gemini:ApiKey" "<key>" --project src/DressShop.Api
```

`Supabase:Url` is in `appsettings.json`; override per environment if needed.

## Running it

```bash
dotnet restore
dotnet build      # 0 warnings, 0 errors
dotnet test       # unit + Api boot test (OpenAPI is served)

dotnet run --project src/DressShop.Api
# -> Scalar UI at https://localhost:5443/scalar
# -> OpenAPI at  https://localhost:5443/openapi/v1.json
```

## Conventions (from ARCHITECTURE.md)

- Thin controllers → `ISender.Send(command/query)` → handler → `IApplicationDbContext`/domain.
  No `IProductRepository`/`ProductService` ceremony for plain CRUD (§45/46).
- Validation (FluentValidation) = "is this request well-formed?"; domain rules =
  "is this operation allowed?".
- `InvalidOperationException` from handlers → `400`; `KeyNotFoundException` → `404`;
  `DomainException` → `400`; `ValidationException` → `400`; unexpected → `500`
  without leaking details.
- Pagination/projection/`AsNoTracking` on reads; explicit transactions only where
  multi-step writes must succeed/fail together (orders, loyalty redeem).

## Multiple NuGet sources / NU1507

If you see `NU1507: ... There are 2 package sources defined ...`, your machine has
an extra global NuGet source (commonly from other IDE tooling). `NuGet.Config` at
the repo root scopes this solution to `nuget.org` only.
