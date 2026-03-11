using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Domain.Services;

public static class ProgressCalculator
{
    public static ProgressProjection Calculate(
        TripRoute route,
        IReadOnlyList<double[]> fullPolyline,
        IEnumerable<TripLogEntry> logEntries,
        DateTime? asOfDate = null
    )
    {
        var totalKm = route.TotalDistanceKm;
        var rawTraveled = logEntries.Sum(e => e.Kilometers);
        var traveledKm = Math.Min(rawTraveled, totalKm);
        var remainingKm = Math.Max(totalKm - traveledKm, 0m);
        var percent = totalKm > 0 ? Math.Round(traveledKm / totalKm * 100m, 2) : 0m;

        var status = DetermineStatus(traveledKm, totalKm);
        var traveledPolyline = SlicePolyline(fullPolyline, traveledKm, totalKm);

        return new ProgressProjection(
            asOfDate ?? DateTime.UtcNow,
            totalKm,
            traveledKm,
            remainingKm,
            percent,
            status,
            fullPolyline,
            traveledPolyline
        );
    }

    private static string DetermineStatus(decimal traveled, decimal total)
    {
        if (traveled <= 0m)
        {
            return "not-started";
        }

        if (traveled >= total)
        {
            return "completed";
        }

        return "in-progress";
    }

    private static IReadOnlyList<double[]> SlicePolyline(
        IReadOnlyList<double[]> polyline,
        decimal traveledKm,
        decimal totalKm
    )
    {
        if (polyline.Count == 0 || traveledKm <= 0m || totalKm <= 0m)
        {
            return [];
        }

        if (traveledKm >= totalKm)
        {
            return polyline;
        }

        var fraction = (double)(traveledKm / totalKm);
        var cutIndex = (int)Math.Ceiling(fraction * (polyline.Count - 1));
        cutIndex = Math.Clamp(cutIndex, 1, polyline.Count);

        return [.. polyline.Take(cutIndex)];
    }
}
