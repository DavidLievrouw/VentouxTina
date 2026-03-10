namespace VentouxTina.Web.Domain.Models;

public record ProgressProjection(
    DateTime AsOfDate,
    decimal TotalDistanceKm,
    decimal TraveledDistanceKm,
    decimal RemainingDistanceKm,
    decimal ProgressPercent,
    string Status,
    IReadOnlyList<double[]> TraveledPolyline
);
