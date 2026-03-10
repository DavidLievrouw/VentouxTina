# Tasks: Ventoux Trip Progress Pagina

**Input**: Design documents from /specs/001-trip-progress-webapp/
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Geautomatiseerde tests zijn in deze proof-of-concept fase niet verplicht. Validatie gebeurt via handmatige story-validaties en smoke checks.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and baseline tooling.

- [ ] T001 Create web project scaffolding in src/VentouxTina.Web/
- [ ] T002 Create test projects in src/VentouxTina.Tests.Unit/ and src/VentouxTina.Tests.Integration/
- [ ] T003 Add projects to solution in src/VentouxTina.slnx
- [ ] T004 [P] Add .NET dependencies in src/VentouxTina.Web/VentouxTina.Web.csproj
- [ ] T005 [P] Add test dependencies in src/VentouxTina.Tests.Unit/VentouxTina.Tests.Unit.csproj and src/VentouxTina.Tests.Integration/VentouxTina.Tests.Integration.csproj
- [ ] T006 [P] Configure CSharpier and Roslynator in src/.config/dotnet-tools.json and src/Directory.Build.props
- [ ] T007 [P] Add baseline app settings and connection placeholders in src/VentouxTina.Web/appsettings.json and src/VentouxTina.Web/appsettings.Development.json
- [ ] T008 [P] Add multi-stage chiseled Dockerfile in src/VentouxTina.Web/Dockerfile
- [ ] T009 [P] Validate and finalize local orchestration in src/docker-compose.yml

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core architecture and infrastructure required before story work.

**CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T010 Create EF Core DbContext and base DB entities in src/VentouxTina.Web/Infrastructure/DataSources/VentouxTinaDbContext.cs
- [ ] T011 Create EF Core entity mappings in src/VentouxTina.Web/Infrastructure/DataSources/Configurations/TripLogEntryConfiguration.cs and src/VentouxTina.Web/Infrastructure/DataSources/Configurations/ProjectContextConfiguration.cs
- [ ] T012 Add initial MariaDB migration in src/VentouxTina.Web/Infrastructure/DataSources/Migrations/
- [ ] T013 [P] Add startup database provisioning and conditional migration execution in src/VentouxTina.Web/Program.cs
- [ ] T014 [P] Add startup migration logging/failure handling in src/VentouxTina.Web/Infrastructure/DataSources/StartupMigrationRunner.cs
- [ ] T015 [P] Implement canonical route model and constants in src/VentouxTina.Web/Domain/Models/TripRoute.cs
- [ ] T016 [P] Implement TripLogEntry, FundraisingGoal, and ProjectContext domain models in src/VentouxTina.Web/Domain/Models/
- [ ] T017 [P] Implement TripCheckpoint domain model in src/VentouxTina.Web/Domain/Models/TripCheckpoint.cs
- [ ] T018 Implement domain validation services for timestamp/kilometers/activity in src/VentouxTina.Web/Domain/Validation/TripLogValidator.cs
- [ ] T019 Implement progress calculation functional core and transient ProgressProjection read model in src/VentouxTina.Web/Domain/Services/ProgressCalculator.cs and src/VentouxTina.Web/Domain/Models/ProgressProjection.cs
- [ ] T020 [P] Add 1-minute in-memory cache policy for progress projection in src/VentouxTina.Web/Domain/Services/CachedProgressService.cs
- [ ] T021 [P] Register EF Core, repositories, and domain services in src/VentouxTina.Web/Program.cs
- [ ] T022 [P] Configure ASP.NET Core rate limiting policies and 429 handling in src/VentouxTina.Web/Program.cs
- [ ] T023 [P] Configure request logging and correlation ID middleware in src/VentouxTina.Web/Program.cs
- [ ] T024 [P] Configure localization defaults for nl-BE in src/VentouxTina.Web/Program.cs
- [ ] T025 Add read-only API skeleton endpoints for progress/log/context in src/VentouxTina.Web/Api/PublicEndpoints.cs
- [ ] T026 [P] Add shared error response contract types in src/VentouxTina.Web/Api/Contracts/ErrorResponse.cs
- [ ] T027 [P] Add one-time PowerShell route seeding script for TripRoute/checkpoints/FundraisingGoal/ProjectContext using OpenRouteService in src/scripts/seed-route.ps1
- [ ] T028 [P] Add generated route payload artifact used by seed script in src/scripts/seed-data/route-checkpoints.json

**Checkpoint**: Foundation and seeding prerequisites are complete. User stories can start.

---

## Phase 3: User Story 1 - Voortgang op de kaart bekijken (Priority: P1) MVP

**Goal**: Toon volledige route en afgelegd segment met correct percentage.

**Independent Test**: Met seeded data in DB kan een bezoeker de route, afgelegd segment en percentage correct zien.

### Validation for User Story 1 (REQUIRED)

- [ ] T029 [P] [US1] Perform manual validation of progress calculation edge cases and document outcomes in specs/001-trip-progress-webapp/quickstart.md
- [ ] T030 [P] [US1] Perform manual validation of route capping and completion status transitions in specs/001-trip-progress-webapp/quickstart.md
- [ ] T031 [P] [US1] Manually validate GET /api/progress response contract against specs/001-trip-progress-webapp/contracts/public-api.yaml
- [ ] T032 [P] [US1] Manually validate map progress payload consistency via API output and rendered UI
- [ ] T033 [P] [US1] Manually verify rendered map line distance equals cumulative TripLogEntry kilometers (capped)

### Implementation for User Story 1

- [ ] T034 [P] [US1] Implement route projection service for traveled polyline in src/VentouxTina.Web/Domain/Services/RouteProjectionService.cs
- [ ] T035 [US1] Implement progress query service using EF Core and functional core in src/VentouxTina.Web/Infrastructure/DataSources/ProgressQueryService.cs
- [ ] T036 [US1] Implement cache-aware progress query flow to avoid DB access on hot reads in src/VentouxTina.Web/Infrastructure/DataSources/ProgressQueryService.cs
- [ ] T037 [US1] Implement GET /api/progress endpoint behavior in src/VentouxTina.Web/Api/PublicEndpoints.cs
- [ ] T038 [P] [US1] Add map host component and Leaflet interop wrapper in src/VentouxTina.Web/Components/Pages/MapProgressSection.razor and src/VentouxTina.Web/wwwroot/js/map.js
- [ ] T039 [US1] Render full route and traveled segment in map component in src/VentouxTina.Web/Components/Pages/MapProgressSection.razor
- [ ] T040 [US1] Add percentage and completion status indicator in src/VentouxTina.Web/Components/Pages/ProgressIndicator.razor
- [ ] T041 [US1] Integrate map and progress section on home page in src/VentouxTina.Web/Components/Pages/Home.razor

**Checkpoint**: US1 fully functional and independently testable.

---

## Phase 4: User Story 2 - Context van het initiatief lezen (Priority: P2)

**Goal**: Toon Nederlandstalige context met duidelijke fundraiserboodschap en doelbedrag.

**Independent Test**: Een bezoeker ziet contexttekst in nl-BE met expliciete vermelding van Klimmen tegen MS en EUR 500.

### Validation for User Story 2 (REQUIRED)

- [ ] T042 [P] [US2] Perform manual validation of context localization rules and Dutch phrasing consistency
- [ ] T043 [P] [US2] Manually validate GET /api/context response contract against specs/001-trip-progress-webapp/contracts/public-api.yaml
- [ ] T044 [P] [US2] Perform manual UI validation of fundraiser context rendering on desktop and mobile

### Implementation for User Story 2

- [ ] T045 [P] [US2] Implement context repository/query service in src/VentouxTina.Web/Infrastructure/DataSources/ProjectContextQueryService.cs
- [ ] T046 [US2] Implement GET /api/context endpoint behavior in src/VentouxTina.Web/Api/PublicEndpoints.cs
- [ ] T047 [US2] Build fundraiser context section component in src/VentouxTina.Web/Components/Pages/FundraiserContextSection.razor
- [ ] T048 [US2] Ensure all visible static labels and section titles are Dutch in src/VentouxTina.Web/Components/Pages/Home.razor
- [ ] T049 [US2] Add Dutch error and empty-state messages for context failures in src/VentouxTina.Web/Components/Pages/FundraiserContextSection.razor

**Checkpoint**: US1 and US2 both independently testable.

---

## Phase 5: User Story 4 - Als backer voortgang volgen (Priority: P2)

**Goal**: Backers kunnen zonder login alle secties volgen via duidelijke navigatie op desktop en mobiel.

**Independent Test**: Een anonieme bezoeker kan op mobiel en desktop via hamburgernavigatie alle secties bereiken.

### Validation for User Story 4 (REQUIRED)

- [ ] T050 [P] [US4] Perform manual validation of navigation state transitions for desktop and mobile layouts
- [ ] T051 [P] [US4] Manually verify anonymous public access to all sections without login
- [ ] T052 [P] [US4] Manually verify hamburger navigation and section-anchor behavior on mobile

### Implementation for User Story 4

- [ ] T053 [P] [US4] Build responsive layout shell with dark mode toggle in src/VentouxTina.Web/Components/Layout/MainLayout.razor and src/VentouxTina.Web/Components/Layout/MainLayout.razor.css
- [ ] T054 [US4] Implement hamburger menu linking to kaart/context/log secties in src/VentouxTina.Web/Components/Layout/NavMenu.razor
- [ ] T055 [US4] Make home sections anchor-addressable for navigation in src/VentouxTina.Web/Components/Pages/Home.razor
- [ ] T056 [US4] Add mobile-first responsive styles for all sections in src/VentouxTina.Web/wwwroot/css/app.css
- [ ] T057 [US4] Enforce Dutch UI copy for navigation controls and dark mode labels in src/VentouxTina.Web/Components/Layout/NavMenu.razor

**Checkpoint**: US4 independently testable by anonymous backers.

---

## Phase 6: User Story 3 - Logboek van activiteiten raadplegen (Priority: P3)

**Goal**: Toon chronologisch logboek uit MariaDB met activity-validatie en robuuste foutafhandeling.

**Independent Test**: Na manuele DB-insert is het logboek zichtbaar op reload of na cache-expiratie en klopt de impact op voortgang.

### Validation for User Story 3 (REQUIRED)

- [ ] T058 [P] [US3] Perform manual validation of TripLogEntry rules for timestamp, kilometers, and activity
- [ ] T059 [P] [US3] Manually validate GET /api/logs response contract against specs/001-trip-progress-webapp/contracts/public-api.yaml
- [ ] T060 [P] [US3] Manually validate DB insert to UI refresh flow with cache expiration behavior

### Implementation for User Story 3

- [ ] T061 [P] [US3] Implement trip log query service with chronological ordering in src/VentouxTina.Web/Infrastructure/DataSources/TripLogQueryService.cs
- [ ] T062 [US3] Implement GET /api/logs endpoint behavior in src/VentouxTina.Web/Api/PublicEndpoints.cs
- [ ] T063 [US3] Build log list component with timestamp/km/activity columns in src/VentouxTina.Web/Components/Pages/TripLogSection.razor
- [ ] T064 [US3] Add Dutch validation and data-error messages in src/VentouxTina.Web/Components/Pages/TripLogSection.razor
- [ ] T065 [US3] Wire log section into home page and recalculate progress projection flow in src/VentouxTina.Web/Components/Pages/Home.razor

**Checkpoint**: All user stories independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final hardening, compliance, and release readiness across stories.

- [ ] T066 [P] Perform manual OpenAPI alignment review for implemented endpoints against specs/001-trip-progress-webapp/contracts/public-api.yaml
- [ ] T067 Verify docker-compose persistence behavior in src/docker-compose.yml and document results in specs/001-trip-progress-webapp/quickstart.md
- [ ] T068 [P] Perform manual container image validation (chiseled base and non-root runtime) using src/VentouxTina.Web/Dockerfile
- [ ] T069 [P] Add throttling observability counters/log fields in src/VentouxTina.Web/Program.cs
- [ ] T070 Perform manual startup validation for provisioning and pending migration execution and document outcomes in specs/001-trip-progress-webapp/quickstart.md
- [ ] T071 Run PoC quality gates (csharpier, roslynator, manual smoke checks) and capture command sequence in specs/001-trip-progress-webapp/quickstart.md
- [ ] T072 Perform final localization audit across all rendered strings and client-side map labels/tooltips; document findings in specs/001-trip-progress-webapp/quickstart.md

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 (Setup): No dependencies.
- Phase 2 (Foundational): Depends on Phase 1 and blocks all user stories.
- Phase 3 (US1): Depends on Phase 2.
- Phase 4 (US2): Depends on Phase 2.
- Phase 5 (US4): Depends on Phase 2, and integrates sections delivered by US1 and US2.
- Phase 6 (US3): Depends on Phase 2.
- Phase 7 (Polish): Depends on completion of all targeted user stories.

### User Story Dependencies

- US1 (P1): Independent after foundational completion.
- US2 (P2): Independent after foundational completion.
- US4 (P2): Independent for public-access/navigation behavior but integrates US1 and US2 sections.
- US3 (P3): Independent after foundational completion.

### Within Each User Story

- Manual validation evidence must be recorded before story sign-off.
- Domain/model/validation changes before service wiring.
- Service wiring before endpoint and UI integration.
- Story is complete only when independent test criteria pass.

### Parallel Opportunities

- Setup tasks marked [P] can run together (T004, T005, T006, T007, T008, T009).
- Foundational tasks marked [P] can run together (T013, T014, T015, T016, T017, T020, T021, T022, T023, T024, T026, T027, T028).
- In each story, [P]-marked validation tasks can run in parallel.
- In each story, [P]-marked model/component tasks can run in parallel where files differ.

---

## Parallel Example: User Story 1

```bash
Task: T029 [US1] Manual validation for progress calculation edge cases
Task: T030 [US1] Manual validation for route capping and status transitions
Task: T031 [US1] Manual contract validation for GET /api/progress
Task: T033 [US1] Manual UI line-distance verification
```

```bash
Task: T034 [US1] Implement route projection service
Task: T038 [US1] Add map host component and Leaflet interop
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 (US1).
3. Validate map progress correctness and percentage behavior.
4. Demo/deploy MVP.

### Incremental Delivery

1. Build foundation once (Phase 1 and Phase 2).
2. Deliver US1 (map progress).
3. Deliver US2 (fundraiser context and Dutch copy baseline).
4. Deliver US4 (public backer navigation and mobile UX).
5. Deliver US3 (database-driven detailed log view).
6. Finish with Phase 7 polish.

### Parallel Team Strategy

1. Team aligns on Phase 1 and Phase 2 together.
2. Then split by story:
   - Developer A: US1
   - Developer B: US2
   - Developer C: US4
   - Developer D: US3
3. Merge after each story checkpoint with documented contract validation.

---

## Notes

- [P] tasks are safe for parallel execution only when file overlap is absent.
- Every user story includes explicit manual validation tasks and independent validation criteria.
- Seeding via OpenRouteService script is a prerequisite before any user story execution.
- Docker compose persistence and throttling are treated as first-class requirements, not optional hardening.
- Keep all visible UI copy in Dutch (Belgium) across components and error states.
