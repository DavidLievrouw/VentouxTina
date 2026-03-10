# Data Model: Ventoux Trip Progress Pagina

## Entity: TripRoute

- Purpose: Canonical representation of the path from Wachtebeke to Mont Ventoux.
- Fields:
  - `routeId` (string, required)
  - `startName` (string, required, fixed: Wachtebeke)
  - `endName` (string, required, fixed: Mont Ventoux)
  - `totalDistanceKm` (decimal, required, > 0)
  - `polyline` (ordered coordinate list, required)
  - `checkpoints` (optional ordered list with cumulative distances)
- Relationships:
  - One `TripRoute` is used by many `TripProgressSnapshot` calculations.

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
  - Many `TripLogEntry` records contribute to one `TripProgressSnapshot`.

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

## Entity: TripProgressSnapshot

- Purpose: Derived read model for UI and API output.
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
  - `progressPercent` = `(traveledDistanceKm / totalDistanceKm) * 100`
  - `remainingDistanceKm` = `max(totalDistanceKm - traveledDistanceKm, 0)`

## State Transitions

## Progress status transitions

- `not-started` -> `in-progress`
  - Trigger: first valid log entry with `kilometers > 0`
- `in-progress` -> `completed`
  - Trigger: `traveledDistanceKm >= totalDistanceKm`
- `completed` -> `in-progress`
  - Trigger: correction entry reduces cumulative kilometers below route total (must be explicitly marked)

## Source ingestion transitions

- `source-valid` -> `source-invalid`
  - Trigger: parse error, schema mismatch, invalid field values
- `source-invalid` -> `source-valid`
  - Trigger: source file fixed and passes validation