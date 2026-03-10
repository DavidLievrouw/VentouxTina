# ============================================================
# Route: Wachtebeke -> Mont Ventoux (foot-walking)
# Generates SQL inserts for trip_checkpoints table
# ============================================================

param(
    # Distance in km between sampled geometry checkpoints
    [double]$SampleIntervalKm = 5.0,

    # Choose checkpoint source: "steps", "geometry", or "both"
    [string]$CheckpointSource = "both"
)

$apiKey       = ""
$tripRouteId  = 1
$profiles     = @("foot-walking", "foot-hiking")  # fallback order

# --- Helper: Haversine distance between two [lon, lat] points ---
function Get-HaversineKm([double]$lon1, [double]$lat1, [double]$lon2, [double]$lat2) {
    $R = 6371.0
    $dLat = ([math]::PI / 180) * ($lat2 - $lat1)
    $dLon = ([math]::PI / 180) * ($lon2 - $lon1)
    $a = [math]::Sin($dLat / 2) * [math]::Sin($dLat / 2) +
         [math]::Cos(([math]::PI / 180) * $lat1) *
         [math]::Cos(([math]::PI / 180) * $lat2) *
         [math]::Sin($dLon / 2) * [math]::Sin($dLon / 2)
    $c = 2 * [math]::Atan2([math]::Sqrt($a), [math]::Sqrt(1 - $a))
    return $R * $c
}

# --- 1. Call the API ---

$headers = @{
    "Authorization" = $apiKey
    "Content-Type"  = "application/json"
}

$body = @{
    coordinates = @(
        @(3.8747, 51.1742),   # Wachtebeke
        @(5.2789, 44.1741)    # Mont Ventoux
    )
    instructions = $true
    elevation    = $true
} | ConvertTo-Json -Depth 3

$response = $null
foreach ($profile in $profiles) {
    $url = "https://api.openrouteservice.org/v2/directions/$profile/geojson"
    Write-Host "Trying profile: $profile ..." -ForegroundColor Cyan
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body
        if ($response.features) {
            Write-Host "Success with profile: $profile" -ForegroundColor Green
            break
        }
    }
    catch {
        Write-Host "Profile $profile failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

if (-not $response -or -not $response.features) {
    Write-Host "Error: No route returned from any profile." -ForegroundColor Red
    exit 1
}

$feature     = $response.features[0]
$coordinates = $feature.geometry.coordinates
$segments    = $feature.properties.segments

Write-Host "Route received: $($coordinates.Count) geometry points" -ForegroundColor Green

# --- 2a. Extract checkpoints from INSTRUCTION STEPS ---

$stepCheckpoints = [System.Collections.ArrayList]::new()
$cumulativeKm = 0.0

if ($CheckpointSource -eq "steps" -or $CheckpointSource -eq "both") {
    foreach ($segment in $segments) {
        foreach ($step in $segment.steps) {
            $idx   = $step.way_points[0]
            $coord = $coordinates[$idx]
            $lon   = [math]::Round([double]$coord[0], 6)
            $lat   = [math]::Round([double]$coord[1], 6)
            $name  = ($step.instruction -replace "'", "''")
            if ($name.Length -gt 200) { $name = $name.Substring(0, 200) }

            [void]$stepCheckpoints.Add([PSCustomObject]@{
                Source              = "step"
                Name                = $name
                CumulativeDistanceKm = [math]::Round($cumulativeKm, 3)
                Latitude            = $lat
                Longitude           = $lon
            })

            $cumulativeKm += ($step.distance / 1000.0)
        }
    }
    Write-Host "Extracted $($stepCheckpoints.Count) step-based checkpoints" -ForegroundColor Green
}

# --- 2b. Sample checkpoints from GEOMETRY at regular intervals ---

$geoCheckpoints = [System.Collections.ArrayList]::new()

if ($CheckpointSource -eq "geometry" -or $CheckpointSource -eq "both") {
    $runningKm      = 0.0
    $nextSampleKm   = 0.0

    for ($i = 0; $i -lt $coordinates.Count; $i++) {
        $lon = [double]$coordinates[$i][0]
        $lat = [double]$coordinates[$i][1]

        if ($i -gt 0) {
            $prevLon = [double]$coordinates[$i - 1][0]
            $prevLat = [double]$coordinates[$i - 1][1]
            $segDist = Get-HaversineKm $prevLon $prevLat $lon $lat
            $runningKm += $segDist
        }

        if ($runningKm -ge $nextSampleKm) {
            $name = "Waypoint at $([math]::Round($runningKm, 1)) km"

            [void]$geoCheckpoints.Add([PSCustomObject]@{
                Source              = "geometry"
                Name                = $name
                CumulativeDistanceKm = [math]::Round($runningKm, 3)
                Latitude            = [math]::Round($lat, 6)
                Longitude           = [math]::Round($lon, 6)
            })

            $nextSampleKm += $SampleIntervalKm
        }
    }

    # Always include the final point
    $lastCoord = $coordinates[$coordinates.Count - 1]
    [void]$geoCheckpoints.Add([PSCustomObject]@{
        Source              = "geometry"
        Name                = "Mont Ventoux (arrival)"
        CumulativeDistanceKm = [math]::Round($runningKm, 3)
        Latitude            = [math]::Round([double]$lastCoord[1], 6)
        Longitude           = [math]::Round([double]$lastCoord[0], 6)
    })

    Write-Host "Extracted $($geoCheckpoints.Count) geometry-sampled checkpoints (every $SampleIntervalKm km)" -ForegroundColor Green
}

# --- 3. Merge and deduplicate ---

$allCheckpoints = [System.Collections.ArrayList]::new()

if ($CheckpointSource -eq "both") {
    # Combine both sources, sort by cumulative distance, remove near-duplicates
    $combined = @($stepCheckpoints) + @($geoCheckpoints) | Sort-Object CumulativeDistanceKm

    $lastKm = -999.0
    foreach ($cp in $combined) {
        # Skip if within 0.5 km of the previous checkpoint
        if ([math]::Abs($cp.CumulativeDistanceKm - $lastKm) -ge 0.5) {
            [void]$allCheckpoints.Add($cp)
            $lastKm = $cp.CumulativeDistanceKm
        }
    }
    Write-Host "After merge & dedup: $($allCheckpoints.Count) checkpoints" -ForegroundColor Green
}
elseif ($CheckpointSource -eq "steps") {
    $allCheckpoints = $stepCheckpoints
}
else {
    $allCheckpoints = $geoCheckpoints
}

# --- 4. Generate SQL ---

$sqlLines = [System.Collections.ArrayList]::new()

[void]$sqlLines.Add("-- Trip checkpoints: Wachtebeke -> Mont Ventoux ($CheckpointSource)")
[void]$sqlLines.Add("-- Generated on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sqlLines.Add("-- Total checkpoints: $($allCheckpoints.Count)")
[void]$sqlLines.Add("")
[void]$sqlLines.Add("INSERT INTO trip_checkpoints (TripRouteId, Name, CumulativeDistanceKm, Latitude, Longitude, OrderIndex)")
[void]$sqlLines.Add("VALUES")

for ($i = 0; $i -lt $allCheckpoints.Count; $i++) {
    $cp    = $allCheckpoints[$i]
    $comma = if ($i -lt $allCheckpoints.Count - 1) { "," } else { ";" }

    $line = "    ($tripRouteId, '$($cp.Name)', $($cp.CumulativeDistanceKm), $($cp.Latitude), $($cp.Longitude), $i)$comma"
    [void]$sqlLines.Add($line)
}

$sql = $sqlLines -join "`n"

# --- 5. Output & save ---

Write-Host ""
Write-Host "============ Generated SQL ============" -ForegroundColor Yellow
Write-Host $sql
Write-Host "=======================================" -ForegroundColor Yellow

$outputFile = "trip_checkpoints_seed.sql"
$sql | Out-File -FilePath $outputFile -Encoding utf8
Write-Host "`nSQL saved to: $outputFile" -ForegroundColor Green

$response | ConvertTo-Json -Depth 20 | Out-File -FilePath "route_raw.geojson" -Encoding utf8
Write-Host "Raw GeoJSON saved to: route_raw.geojson" -ForegroundColor Green

Write-Host "`nDone! Total checkpoints: $($allCheckpoints.Count)" -ForegroundColor Cyan
