<!--
Sync Impact Report
- Version change: N/A -> 1.0.0
- Modified principles:
	- Principle 1 (template) -> I. Functional Core, Imperative Shell
	- Principle 2 (template) -> II. Canonical Trip Progress Model
	- Principle 3 (template) -> III. Test-First Quality Gates
	- Principle 4 (template) -> IV. Container-First Reproducible Delivery
	- Principle 5 (template) -> V. Modern Standards and Transparency
- Added sections:
	- Technical Standards & Constraints
	- Delivery Workflow & Quality Gates
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ .specify/templates/plan-template.md
	- ✅ .specify/templates/spec-template.md
	- ✅ .specify/templates/tasks-template.md
	- ⚠ pending .specify/templates/commands/*.md (directory not present)
- Follow-up TODOs:
	- None
-->

# VentouxTina Constitution

## Core Principles

### I. Functional Core, Imperative Shell
All trip-domain logic MUST be implemented as deterministic, side-effect-free functions.
Input/output boundaries (HTTP, persistence, map providers, clocks, logging) MUST be isolated in thin
adapters that call the functional core. Shared state, hidden mutation, and business logic in controllers
or UI components are prohibited.

Rationale: Functional architecture keeps route progress calculations predictable, testable, and easier to
reason about as entries grow.

### II. Canonical Trip Progress Model
The system MUST treat the route from Wachtebeke (Belgium) to Mont Ventoux (France) as a canonical,
ordered path with a single total distance baseline. Every log entry MUST include traveled kilometers,
timestamp; cumulative progress MUST be monotonic unless a correction entry is
explicitly marked and auditable. The map view and log list MUST be generated from the same canonical
dataset to prevent divergence.

Rationale: A single source of truth prevents inconsistent public progress reporting.

### III. Test-First Quality Gates
Behavior-changing work MUST begin with failing automated tests for the affected domain behavior.
At minimum, each feature MUST include unit tests for functional core logic and integration tests for
API-to-storage and API-to-map projection paths. Pull requests MUST not be merged with failing tests,
reduced coverage in changed areas, or missing regression tests for fixed defects.

Rationale: Progress visualization is user-facing and trust-sensitive; regressions must be caught early.

### IV. Container-First Reproducible Delivery
The application MUST run as a Docker container for all shared environments. Local development,
CI validation, and production deployment MUST use the same container build definition, with
configuration injected via environment variables and secrets management. Releases MUST be immutable,
traceable to commit SHA, and rollback-capable.

Rationale: Reproducible containerized delivery minimizes "works on my machine" failures.

### V. Modern Standards and Transparency
Code MUST target current stable language/runtime standards and current maintained dependencies.
Deprecated APIs and unmaintained packages are prohibited unless explicitly approved with a
time-boxed remediation plan. Public-facing progress endpoints MUST expose clear, documented schemas,
and system behavior changes MUST be recorded in changelogs.

Rationale: Modern standards improve security and maintainability while transparency reinforces user trust.

## Technical Standards & Constraints

- Product scope MUST remain a web application that publishes trip progress for public consultation.
- The core user experience MUST include:
	- a map visualization of current trip progress from Wachtebeke to Mont Ventoux
	- a chronological list of progress log entries
- Domain inputs MUST validate non-negative distances and valid timestamps.
- API contracts MUST be versioned when breaking schema changes are introduced.
- Structured logging (JSON or equivalent) MUST include correlation IDs for request tracing.
- Performance objective: render primary progress view in under 2 seconds at p95 under expected load.

## Delivery Workflow & Quality Gates

- Every feature spec MUST map requirements to the five core principles before implementation begins.
- Implementation plans MUST include an explicit constitution check with pass/fail evidence.
- Task lists MUST include work items for tests, container verification, and observability updates.
- Code review requires at least one approval and explicit confirmation that constitution gates pass.
- CI MUST execute linting, unit tests, integration tests, and container image build validation.
- Releases MUST publish image tag, commit SHA, and high-level change summary.
- When a PoC exception is used, CI MUST be marked as deferred with rationale, owner, and planned reintroduction milestone.

## Governance
This constitution supersedes conflicting team conventions and template defaults.

- Amendment process: proposed changes MUST include rationale, impact analysis, and template sync
	updates in the same change set.
- Versioning policy: this constitution follows semantic versioning.
	- MAJOR: incompatible governance changes or principle removals/redefinitions.
	- MINOR: new principle/section or materially expanded guidance.
	- PATCH: clarifications, wording improvements, and non-semantic refinements.
- Compliance review: every plan, spec, and task artifact MUST include constitution alignment checks;
	pull request reviews MUST verify compliance before approval.
- Exceptions: temporary exceptions MUST be documented with owner, expiry date, and mitigation plan.

**Version**: 1.0.0 | **Ratified**: 2026-03-10 | **Last Amended**: 2026-03-10
