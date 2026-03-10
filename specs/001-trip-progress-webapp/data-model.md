# Data Model: Ventoux Trip Progress Pagina

## Entity: TripRoute

- Purpose: Canonical representation of the path from Wachtebeke to Mont Ventoux.
- Fields:
  - `routeId` (string, required)
  - `startName` (string, required, fixed: Wachtebeke)
  - `endName` (string, required, fixed: Mont Ventoux)
  - `totalDistanceKm` (decimal, required, > 0)
  - `polyline` (ordered coordinate list, required)
  - `checkpoints` (ordered list with cumulative distances)
- Seed source:
  - One-time PowerShell script fetches route geometry/checkpoints from a free route planner API.
- Relationships:
  - One `TripRoute` is used by many runtime progress projections.

## Entity: TripLogEntry

- Purpose: One manually curated activity line used for progress calculation.
- Fields:
  - `entryId` (string, required, unique in source)
  - `date` (date, required)
  - `kilometers` (decimal, required, >= 0)
  - `activity` (enum, required: `running`, `cycling`, `walking`)
  - `sourceLine` (integer, optional for diagnostics)
  - `isCorrection` (boolean, optional, default false)
- Validation rules:
  - `date` must parse correctly and be within realistic bounds.
  - `kilometers` must be non-negative.
  - `activity` must match allowed values.
  - Duplicate `entryId` values are invalid.
- Relationships:
  - Many `TripLogEntry` records contribute to one runtime progress projection.

## Entity: FundraisingGoal

- Purpose: Charity context metadata for page messaging.
- Fields:
  - `organizationName` (string, required, fixed: Klimmen tegen MS)
  - `goalAmountEur` (decimal, required, fixed initial value: 500)
  - `isFundraiser` (boolean, required, true)
  - `audience` (string, required, includes backers)

## Entity: ProjectContext

- Purpose: Localized static text block shown between map and logs.
- Fields:
  - `locale` (string, required, fixed: nl-BE)
  - `headline` (string, required)
  - `bodyText` (string, required)
  - `fundraisingGoalText` (string, required, includes EUR 500 mention)

## Read Model: ProgressProjection (Transient, Non-Persistent)

- Purpose: Derived in-memory projection for UI and API output.
- Persistence: Not stored in database and never manually maintained.
- Fields:
  - `asOfDate` (date-time, required)
  - `totalDistanceKm` (decimal, required)
  - `traveledDistanceKm` (decimal, required, >= 0)
  - `remainingDistanceKm` (decimal, required, >= 0)
  - `progressPercent` (decimal, required, 0..100)
  - `status` (enum: `not-started`, `in-progress`, `completed`)
  - `traveledPolyline` (ordered coordinate list, required)
- Derivation rules:
  - `traveledDistanceKm` = sum(valid `TripLogEntry.kilometers`)
  - `traveledDistanceKm` capped at `totalDistanceKm`
  - `traveledPolyline` length corresponds to `traveledDistanceKm` along canonical route geometry
  - `progressPercent` = `(traveledDistanceKm / totalDistanceKm) * 100`
  - `remainingDistanceKm` = `max(totalDistanceKm - traveledDistanceKm, 0)`
  - Computed projection is cached in-memory for 1 minute.

## State Transitions

## Progress status transitions

- `not-started` -> `in-progress`
  - Trigger: first valid log entry with `kilometers > 0`
- `in-progress` -> `completed`
  - Trigger: `traveledDistanceKm >= totalDistanceKm`
- `completed` -> `in-progress`
  - Trigger: correction entry reduces cumulative kilometers below route total (must be explicitly marked)

## Data ingestion transitions

- `records-valid` -> `records-invalid`
  - Trigger: invalid manually entered DB records (date/kilometers/activity constraints)
- `records-invalid` -> `records-valid`
  - Trigger: DB records are corrected and pass validation rules

## Startup data lifecycle transitions

- `schema-pending` -> `schema-ready`
  - Trigger: application startup provisions database and applies pending EF Core migrations
- `route-not-seeded` -> `route-seeded`
  - Trigger: one-time PowerShell route seed script writes `TripRoute` and checkpoints
- `projection-cache-miss` -> `projection-cache-hit`
  - Trigger: first projection computed and stored in-memory cache with 1-minute TTL