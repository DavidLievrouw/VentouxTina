# Quickstart: Ventoux Trip Progress Pagina

## Prerequisites

- .NET 10 SDK
- Docker (with BuildKit)
- Node/npm only if chosen UI toolchain requires asset build steps

## 1. Create solution/project structure in src

```powershell
Set-Location src
dotnet new blazor -n VentouxTina.Web --interactivity None
dotnet sln VentouxTina.slnx add .\VentouxTina.Web\VentouxTina.Web.csproj
dotnet new xunit -n VentouxTina.Tests.Unit
dotnet new xunit -n VentouxTina.Tests.Integration
dotnet sln VentouxTina.slnx add .\VentouxTina.Tests.Unit\VentouxTina.Tests.Unit.csproj
dotnet sln VentouxTina.slnx add .\VentouxTina.Tests.Integration\VentouxTina.Tests.Integration.csproj
```

## 2. Add required packages

```powershell
Set-Location src\VentouxTina.Web
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.AspNetCore.RateLimiting

Set-Location ..\VentouxTina.Tests.Unit
dotnet add package Shouldly
dotnet add package FakeItEasy

Set-Location ..\VentouxTina.Tests.Integration
dotnet add package Shouldly
dotnet add package FakeItEasy
```

## 3. Local run

```powershell
Set-Location src\VentouxTina.Web
dotnet run
```

Browse to `http://localhost:5000` (or assigned port) and verify:

- Map section loads full route and traveled segment
- Context section includes Klimmen tegen MS fundraiser text and EUR 500 target
- Log section shows entries loaded from MariaDB
- Dark mode and hamburger menu work on desktop/mobile
- Alle zichtbare UI-teksten zijn in het Nederlands (Belgie)

## 4. Test and quality checks

```powershell
Set-Location src
dotnet test
dotnet tool restore
dotnet csharpier .
dotnet roslynator analyze .
```

## 5. Docker image (chiseled)

Example build command from repository root:

```powershell
docker build -f src\VentouxTina.Web\Dockerfile -t ventouxtina:dev .
```

Example run command:

```powershell
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e ConnectionStrings__MariaDb="Server=localhost;Port=3306;Database=ventouxtina;User=ventouxtina;Password=ventouxtina_dev_pw;" \
  ventouxtina:dev
```

## 6. Docker Compose for local machine

Use the compose file at `src/docker-compose.yml`:

```powershell
Set-Location src
docker compose up -d --build
docker compose ps
```

Stop services:

```powershell
docker compose down
```

Stop services and remove database state volume:

```powershell
docker compose down -v
```

Expected Docker characteristics:

- Multi-stage build
- Runtime image based on `mcr.microsoft.com/dotnet/aspnet:10.0-jammy-chiseled` (or pinned digest)
- Non-root runtime user
- Small final image footprint suitable for low-cost hosting tiers

## 7. Smoke checks

- `GET /` returns rendered page with three sections
- `GET /api/progress` returns valid progress payload
- Rate limit policy returns `429` when threshold exceeded
- UI remains usable on mobile viewport widths
- MariaDB-data blijft behouden na `docker compose down` en opnieuw `docker compose up`