<#
.SYNOPSIS
    One-time route seeding script for VentouxTina.
    Fetches route data from OpenRouteService and seeds TripRoute, TripCheckpoints,
    FundraisingGoal and ProjectContext into the MariaDB database.

.PARAMETER ConnectionString
    MariaDB connection string. Defaults to development connection string.

.PARAMETER OrsApiKey
    OpenRouteService API key. Required for fetching actual route geometry.

.PARAMETER DryRun
    If set, writes the route data to route-checkpoints.json without inserting into DB.

.EXAMPLE
    .\seed-route.ps1 -OrsApiKey "your-api-key-here" -DryRun
    .\seed-route.ps1 -OrsApiKey "your-api-key-here" -ConnectionString "Server=localhost;Port=3306;Database=ventouxtina;User=ventouxtina;Password=ventouxtina_dev_pw;"
#>
param(
    [string]$ConnectionString = "Server=localhost;Port=3306;Database=ventouxtina;User=ventouxtina;Password=ventouxtina_dev_pw;",
    [string]$OrsApiKey = "",
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SeedDataPath = Join-Path $ScriptDir "seed-data\route-checkpoints.json"

# OpenRouteService endpoint for directions
$OrsBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car"

# Wachtebeke: 51.1636, 3.8553
# Mont Ventoux: 44.1742, 5.2789
$StartCoord = @(3.8553, 51.1636)   # [lon, lat]
$EndCoord   = @(5.2789, 44.1742)   # [lon, lat]

function Get-RouteFromOrs {
    param([string]$ApiKey)

    Write-Host "Fetching route from OpenRouteService..."

    $headers = @{
        "Authorization" = $ApiKey
        "Content-Type"  = "application/json"
        "Accept"        = "application/json, application/geo+json"
    }

    $body = @{
        coordinates = @($StartCoord, $EndCoord)
    } | ConvertTo-Json -Depth 5

    $response = Invoke-RestMethod -Uri $OrsBaseUrl -Method POST -Headers $headers -Body $body
    return $response
}

function Convert-OrsPolylineToLatLon {
    param($EncodedOrRaw)

    # ORS returns geometry as GeoJSON LineString
    if ($EncodedOrRaw.type -eq "LineString") {
        # coordinates are [lon, lat] — convert to [lat, lon] for Leaflet
        return $EncodedOrRaw.coordinates | ForEach-Object { @($PSItem[1], $PSItem[0]) }
    }

    return @()
}

function Build-SeedPayload {
    param($RouteResponse)

    $geometry = $RouteResponse.routes[0].geometry
    $summary  = $RouteResponse.routes[0].summary
    $totalKm  = [Math]::Round($summary.distance / 1000, 3)

    $polyline = Convert-OrsPolylineToLatLon -EncodedOrRaw $geometry

    $checkpoints = @(
        @{ name = "Wachtebeke";   cumulativeDistanceKm = 0;        orderIndex = 0; lat = 51.1636; lon = 3.8553 }
        @{ name = "Gent";         cumulativeDistanceKm = 15;       orderIndex = 1; lat = 51.0534; lon = 3.7196 }
        @{ name = "Brussel";      cumulativeDistanceKm = 55;       orderIndex = 2; lat = 50.8503; lon = 4.3517 }
        @{ name = "Namen";        cumulativeDistanceKm = 115;      orderIndex = 3; lat = 50.4674; lon = 4.8719 }
        @{ name = "Luxemburg";    cumulativeDistanceKm = 210;      orderIndex = 4; lat = 49.6117; lon = 6.1319 }
        @{ name = "Metz";         cumulativeDistanceKm = 280;      orderIndex = 5; lat = 49.1193; lon = 6.1757 }
        @{ name = "Lyon";         cumulativeDistanceKm = 490;      orderIndex = 6; lat = 45.7640; lon = 4.8357 }
        @{ name = "Valence";      cumulativeDistanceKm = 580;      orderIndex = 7; lat = 44.9334; lon = 4.8924 }
        @{ name = "Mont Ventoux"; cumulativeDistanceKm = $totalKm; orderIndex = 8; lat = 44.1742; lon = 5.2789 }
    )

    return @{
        routeId        = "wachtebeke-mont-ventoux"
        startName      = "Wachtebeke"
        endName        = "Mont Ventoux"
        totalDistanceKm = $totalKm
        polyline       = $polyline
        checkpoints    = $checkpoints
        fundraisingGoal = @{
            organizationName = "Klimmen tegen MS"
            goalAmountEur    = 500
            isFundraiser     = $true
            audience         = "backers"
        }
        projectContext = @{
            locale               = "nl-BE"
            headline             = "Tina fietst van Wachtebeke naar Mont Ventoux voor Klimmen tegen MS"
            bodyText             = "Volg de voortgang van Tina op haar tocht van Wachtebeke naar de top van de Mont Ventoux. Elke kilometer telt voor het goede doel 'Klimmen tegen MS'."
            fundraisingGoalText  = "Steun Klimmen tegen MS – doelstelling EUR 500"
        }
    }
}

# ── Main ──────────────────────────────────────────────────────────────────────

if (-not $OrsApiKey -and -not $DryRun) {
    Write-Warning "No -OrsApiKey provided. Using placeholder data from $SeedDataPath."
    $payload = Get-Content $SeedDataPath -Raw | ConvertFrom-Json
} elseif ($OrsApiKey) {
    $orsResponse = Get-RouteFromOrs -ApiKey $OrsApiKey
    $payload = Build-SeedPayload -RouteResponse $orsResponse
    $payload | ConvertTo-Json -Depth 10 | Set-Content $SeedDataPath
    Write-Host "Route data written to $SeedDataPath"
} else {
    Write-Host "(DryRun) Using existing data from $SeedDataPath"
    $payload = Get-Content $SeedDataPath -Raw | ConvertFrom-Json
}

if ($DryRun) {
    Write-Host "DryRun: Seed payload ready (not inserting into DB)."
    $payload | ConvertTo-Json -Depth 10
    exit 0
}

# ── Insert into MariaDB using MySqlConnector ──────────────────────────────────
Write-Host "Seeding database at: $ConnectionString"

# We use dotnet-script or inline C# via dotnet run is impractical here;
# instead use mysql CLI if available, otherwise instruct the user.
$mysqlAvailable = Get-Command mysql -ErrorAction SilentlyContinue

if (-not $mysqlAvailable) {
    Write-Warning "mysql CLI not found. To seed manually, connect to the database and run:"
    Write-Warning "  - Insert TripRoute with polyline JSON"
    Write-Warning "  - Insert TripCheckpoints"
    Write-Warning "  - Insert FundraisingGoal"
    Write-Warning "  - Insert ProjectContext"
    Write-Warning "Or run this script from within the Docker environment where mysql is available."
    exit 1
}

# Parse connection string for mysql CLI
$csMatch = [regex]::Match($ConnectionString, "Server=(?<s>[^;]+);Port=(?<p>[^;]+);Database=(?<d>[^;]+);User=(?<u>[^;]+);Password=(?<pw>[^;]+)")
$server   = $csMatch.Groups["s"].Value
$port     = $csMatch.Groups["p"].Value
$database = $csMatch.Groups["d"].Value
$user     = $csMatch.Groups["u"].Value
$password = $csMatch.Groups["pw"].Value

$polylineJson = ($payload.polyline | ConvertTo-Json -Compress -Depth 5) -replace "'", "''"
$totalKm      = $payload.totalDistanceKm

$sql = @"
INSERT IGNORE INTO trip_routes (route_id, start_name, end_name, total_distance_km, polyline_json)
VALUES ('$($payload.routeId)', '$($payload.startName)', '$($payload.endName)', $totalKm, '$polylineJson');

SET @route_id = (SELECT id FROM trip_routes WHERE route_id = '$($payload.routeId)');

"@

foreach ($cp in $payload.checkpoints) {
    $sql += "INSERT IGNORE INTO trip_checkpoints (trip_route_id, name, cumulative_distance_km, latitude, longitude, order_index) VALUES (@route_id, '$($cp.name)', $($cp.cumulativeDistanceKm), $($cp.lat), $($cp.lon), $($cp.orderIndex));`n"
}

$sql += @"

INSERT IGNORE INTO fundraising_goals (organization_name, goal_amount_eur, is_fundraiser, audience)
VALUES ('$($payload.fundraisingGoal.organizationName)', $($payload.fundraisingGoal.goalAmountEur), $([int]$payload.fundraisingGoal.isFundraiser), '$($payload.fundraisingGoal.audience)');

INSERT IGNORE INTO project_contexts (locale, headline, body_text, fundraising_goal_text)
VALUES ('$($payload.projectContext.locale)', '$($payload.projectContext.headline)', '$($payload.projectContext.bodyText)', '$($payload.projectContext.fundraisingGoalText)');
"@

$sql | & mysql -h $server -P $port -u $user "-p$password" $database
Write-Host "Seeding complete."
