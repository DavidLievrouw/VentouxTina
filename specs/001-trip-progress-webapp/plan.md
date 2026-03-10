# Implementation Plan: Ventoux Trip Progress Pagina

**Branch**: `001-trip-progress-webapp` | **Date**: 2026-03-10 | **Spec**: `specs/001-trip-progress-webapp/spec.md`
**Input**: Feature specification from `/specs/001-trip-progress-webapp/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Build a single-page .NET 10 Blazor Server-rendered web application in `src` that publicly shows Tina's
trip progress from Wachtebeke to Mont Ventoux for the Klimmen tegen MS fundraiser. The UI (Dutch,
Belgium) includes a route map with traveled overlay and percentage indicator, fundraiser context with
EUR 500 target, and a chronological activity log read from MariaDB where records are manually maintained.
Technical approach: functional-core domain services, Entity Framework Core with MariaDB provider,
Leaflet/OpenStreetMap map rendering, responsive dark-mode UX with hamburger navigation, built-in
ASP.NET Core rate limiting, and Docker delivery with chiseled runtime images plus local docker-compose orchestration.
Database provisioning and conditional EF Core migrations run on startup; route/checkpoint seed data is
loaded once via a PowerShell script using OpenRouteService (free tier). Progress is projected in-memory and
cached for 1 minute, with freshness accepted after cache expiration. The rendered map progress line is constrained to cumulative logged distance,
never beyond route end.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# on .NET 10 (ASP.NET Core / Blazor Server rendering)
**Primary Dependencies**: ASP.NET Core Blazor, Entity Framework Core, Pomelo.EntityFrameworkCore.MySql (MariaDB provider), Microsoft.Extensions.Caching.Memory, Leaflet JS + OpenStreetMap tiles, optional MudBlazor UI controls, ASP.NET Core Rate Limiting middleware
**Storage**: MariaDB for trip logs (timestamp-based), route/checkpoints, and context metadata; no persisted trip progress snapshot table
**Testing**: xUnit, Shouldly, FakeItEasy; bUnit for Razor component behavior where valuable
**Target Platform**: Linux container hosting (public internet), desktop and mobile browsers
**Project Type**: Web application (server-rendered UI + read-only API endpoints)
**Performance Goals**: Primary page render under 2s p95; API responses under 200ms p95 for cached reads; progress read paths use 1-minute in-memory cache to reduce DB load
**Constraints**: Must run from `src`; all visible UI copy in Dutch (Belgium); dark mode, mobile hamburger navigation, and throttling are explicit functional requirements; map progress line must match cumulative logged km (capped at total route distance); formatting via CSharpier and Roslynator recommendations
**Scale/Scope**: Public read-mostly traffic; single route, single-page experience, manually maintained activity log records in MariaDB, startup provisioning and migration execution, no authentication and no payment processing

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] Functional core boundary is explicit: business rules are pure functions, side effects isolated.
- [x] Canonical trip model defined: single route baseline, monotonic cumulative progress rules,
      correction-entry strategy, and shared dataset for map + log views.
- [x] Test-first plan documented: failing tests first, plus unit and integration coverage strategy.
- [x] Container-first delivery plan documented: Docker build, env/secrets strategy, immutable tagging.
- [x] Modern standards confirmed: current stable language/runtime and maintained dependencies.

## Project Structure

### Documentation (this feature)

```text
specs/001-trip-progress-webapp/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
src/
├── docker-compose.yml
├── VentouxTina.Web/
│   ├── Components/
│   │   ├── Layout/
│   │   └── Pages/
│   ├── Domain/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Validation/
│   ├── Infrastructure/
│   │   ├── DataSources/
│   │   └── Mapping/
│   ├── Api/
│   ├── wwwroot/
│   ├── appsettings.json
│   └── Program.cs
├── VentouxTina.Tests.Unit/
├── VentouxTina.Tests.Integration/
└── VentouxTina.slnx
```

**Structure Decision**: Single web project and test projects under `src` are selected to satisfy
the repository constraint while separating functional core, infrastructure adapters, UI, and tests.
This supports constitution-aligned boundaries and low operational complexity.

Local development orchestration is provided through `src/docker-compose.yml` with web + MariaDB
services and a persistent MariaDB volume.

## Post-Design Constitution Check

- [x] Functional core boundary remains explicit in data model and service contracts.
- [x] Canonical trip model is represented with route baseline, monotonic progress, and correction handling.
- [x] Test-first strategy captured with xUnit + Shouldly + FakeItEasy across unit/integration/component tests.
- [x] Container-first delivery is concrete with chiseled runtime image and immutable tagging guidance.
- [x] Modern standards/tooling are explicit (.NET 10, CSharpier formatting, Roslynator guidance).

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitution violations identified.
