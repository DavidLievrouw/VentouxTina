# Phase 0 Research: Ventoux Trip Progress Pagina

## Decision 1: Web framework and render model

- Decision: Use .NET 10 Blazor with server-side rendering as the primary delivery model.
- Rationale: The product is a single public page with mostly read-only interactions; server-side rendering
  provides fast first paint, simple hosting topology, and no heavy client bundle requirement.
- Alternatives considered:
  - Interactive Blazor Server with persistent SignalR circuits: rejected for avoidable server resource usage.
  - Blazor WebAssembly: rejected because it adds client payload and complexity with limited user value here.

## Decision 2: Map technology

- Decision: Use Leaflet with OpenStreetMap tiles for route visualization, with optional abstraction to swap
  map providers later.
- Rationale: It avoids mandatory paid API usage, supports full-route plus traveled-segment overlays,
  and keeps hosting cost low while meeting public-read requirements.
- Alternatives considered:
  - Google Maps: supported as fallback, but rejected initially due to key management and variable usage costs.
  - Proprietary map SDKs: rejected because they do not add essential value for this use case.

## Decision 3: Canonical route and progress calculation model

- Decision: Keep a static canonical route definition with fixed total distance and compute progress from the
  cumulative sum of validated log entries.
- Rationale: A single source of truth satisfies constitution requirements and keeps map and log in sync.
  Traveled distance is capped at total route distance for a stable 0-100% indicator.
- Alternatives considered:
  - Recalculate route distance dynamically from mapping APIs: rejected for cost/latency variability.
  - Trust precomputed percentage values from source file: rejected to prevent drift and data inconsistencies.

## Decision 4: Log source format handling

- Decision: Support both JSON and YAML input files with a deterministic priority rule and strict validation.
- Rationale: Matches the manual-maintenance requirement while keeping operational flexibility.
  Validation prevents negative distances, invalid dates, and unsupported activity values.
- Alternatives considered:
  - JSON-only: rejected because YAML support is explicitly requested.
  - Database storage: rejected because the requested workflow is manual file updates.

## Decision 5: UI stack, modern look, and responsive navigation

- Decision: Use modern component styling with dark mode and a mobile-first responsive layout that includes
  a hamburger menu for section navigation.
- Rationale: Meets explicit UX requirements (modern appearance, dark mode, mobile friendliness) and keeps
  all content accessible on small screens.
- Alternatives considered:
  - Plain default styling: rejected because it does not meet the requested modern visual quality.
  - Desktop-first layout with shrinking behavior: rejected due to poor mobile readability.

## Decision 6: Throttling strategy for hosting-cost control

- Decision: Apply ASP.NET Core built-in rate limiting with per-IP partitioning for public endpoints,
  low queue limits, and explicit 429 behavior.
- Rationale: Protects compute resources and prevents expensive spikes while keeping implementation simple
  and observable.
- Alternatives considered:
  - No throttling: rejected because it conflicts with cost-control requirement.
  - External gateway-only throttling: rejected as sole control because app-level limits are still valuable.

## Decision 7: Test strategy and quality tooling

- Decision: Use xUnit for test framework, Shouldly for assertions, FakeItEasy for fakes, and adopt a
  failing-test-first workflow for behavior changes.
- Rationale: Satisfies explicit testing constraints and aligns with constitution quality gates.
- Alternatives considered:
  - NUnit/MSTest: rejected because they do not match specified toolchain.
  - Mock-heavy integration-first approach: rejected in favor of focused functional-core unit tests first.

## Decision 8: Formatting and static analysis standards

- Decision: Enforce CSharpier formatting and Roslynator analyzer recommendations in local/CI checks.
- Rationale: Keeps codebase consistent and modern while reducing style churn in pull requests.
- Alternatives considered:
  - Dotnet format only: rejected because CSharpier is explicitly requested.
  - Analyzer-free workflow: rejected due to maintainability risk.

## Decision 9: Containerization and runtime image

- Decision: Build with multi-stage Dockerfile and run on .NET 10 chiseled ASP.NET runtime image.
- Rationale: Chiseled images reduce size and attack surface, lowering hosting footprint and transfer costs.
- Alternatives considered:
  - Full ASP.NET runtime image: rejected due to larger size.
  - Alpine image path: rejected as default to avoid globalization surprises for Dutch locale content.

## Decision 10: Observability and operational controls

- Decision: Emit structured logs and include rate-limit rejection metrics, with environment-driven config.
- Rationale: Supports cost monitoring and debugging without introducing heavy observability infrastructure.
- Alternatives considered:
  - Minimal logging only: rejected because throttling behavior needs auditable visibility.
  - Full distributed tracing stack from day one: deferred as optional enhancement.