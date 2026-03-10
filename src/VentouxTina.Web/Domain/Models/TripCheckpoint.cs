namespace VentouxTina.Web.Domain.Models;

public class TripCheckpoint
{
    public int Id { get; set; }
    public int TripRouteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CumulativeDistanceKm { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int OrderIndex { get; set; }

    public TripRoute? TripRoute { get; set; }
}
