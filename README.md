# DressShop.Api

Scaffold for a dress-shop backend, following the Clean Architecture +
Vertical Slice conventions in `ARCHITECTURE.md`.

## What's in this commit (Task 1 — project setup only)

This is deliberately just the skeleton: four `src` projects wired together
with the correct dependency direction, two empty test projects, centralized
package version management, and a working `/health` endpoint. **No entities,
no controllers, no features yet** — that's Task 2, once you tell me what the
first vertical slice should be (e.g. `CreateProduct`, `RegisterCustomer`).

```
DressShop/
├── DressShop.sln
├── global.json                    # pins the SDK version
├── Directory.Build.props          # shared compiler settings (nullable, warnings-as-errors)
├── Directory.Packages.props       # central NuGet package versions
├── src/
│   ├── DressShop.Domain/          # no dependencies, no NuGet packages
│   ├── DressShop.Application/     # -> Domain. MediatR + FluentValidation wiring
│   ├── DressShop.Infrastructure/  # -> Application, Domain. EF Core DbContext
│   └── DressShop.Api/             # -> Application, Infrastructure. Program.cs, Swagger, global exception handler
└── tests/
    ├── DressShop.UnitTests/       # -> Domain, Application
    └── DressShop.IntegrationTests/ # -> Api (WebApplicationFactory)
```

## Assumptions made — flag these if wrong

1. **.NET 10 (LTS)** — originally scaffolded against .NET 8, retargeted after
   confirming the SDKs actually installed were 9.0.317 / 10.0.400. .NET 10 is
   the current LTS (even major versions — 6, 8, 10 — get long-term support;
   odd ones like 9 are short-term). `global.json` now pins `10.0.100` with
   `rollForward: latestFeature`, which will resolve to `10.0.400` or newer.
2. **SQL Server** as the EF Core provider (`appsettings.json` connection string
   targets LocalDB). If you'd rather use PostgreSQL or SQLite for local dev,
   that's a one-line swap in `Infrastructure.csproj` and
   `Infrastructure/DependencyInjection.cs` — say the word and I'll redo it.
3. **MediatR** for the command/query dispatch (`ISender.Send(...)`), matching
   the pattern in `ARCHITECTURE.md` section 6. This is a real dependency
   decision, not free — MediatR adds an indirection layer and (in v12+) a
   commercial license consideration for very large organizations. For a
   learning project it's the standard, well-documented way to implement this
   pattern; a hand-rolled dispatcher is a legitimate alternative if you want
   one less dependency.
4. Controllers, not Minimal API endpoints — easier to navigate as a beginner,
   and what the architecture doc's examples assume.

## Running it

```bash
# restore & build
dotnet restore
dotnet build

# apply migrations once you have entities + your first DbSet
dotnet ef migrations add InitialCreate -p src/DressShop.Infrastructure -s src/DressShop.Api
dotnet ef database update -p src/DressShop.Infrastructure -s src/DressShop.Api

# run the API
dotnet run --project src/DressShop.Api
# -> Swagger UI at https://localhost:5443/swagger
# -> Health check at https://localhost:5443/health

# run tests
dotnet test
```

> This scaffold was generated without access to the .NET SDK (sandboxed
> environment with no NuGet access), so it has **not** been through
> `dotnet build` locally. The project files are hand-written to match exactly
> what `dotnet new` + `dotnet add reference` would produce. Run `dotnet build`
> as your first step — if something doesn't compile, it's most likely a NuGet
> package version in `Directory.Packages.props` that's since been superseded;
> bump it and retry.

## Multiple NuGet sources / NU1507

If you see `NU1507: ... There are 2 package sources defined ...`, that's not
this scaffold's fault — it means your machine has more than one NuGet source
configured globally (commonly added by other IDE tooling, e.g. DevExpress),
and Central Package Management refuses to guess which source a pinned
version should come from. `NuGet.Config` at the repo root fixes this by
scoping this solution to `nuget.org` only, without touching your global
NuGet settings. If you added other private feeds, add a `<package pattern>`
entry per feed instead of relying on the `*` wildcard.

## Why no repositories / services yet

Per `ARCHITECTURE.md` section 45/46: don't add `IProductRepository` /
`ProductService` layers until a feature actually needs them. `AppDbContext` is
injected directly into feature handlers for simple CRUD. Introduce a
repository or service only when it represents real reusable logic (pricing,
tax, inventory allocation, etc.), not as a default pattern.
