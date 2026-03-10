namespace VentouxTina.Web.Domain.Models;

public class TripRoute
{
    public int Id { get; set; }
    public string RouteId { get; set; } = string.Empty;
    public string StartName { get; set; } = "Wachtebeke";
    public string EndName { get; set; } = "Mont Ventoux";
    public decimal TotalDistanceKm { get; set; }

    /// <summary>Ordered coordinate pairs stored as JSON: [[lat,lon],...]</summary>
    public string PolylineJson { get; set; } = "[]";

    public ICollection<TripCheckpoint> Checkpoints { get; set; } = new List<TripCheckpoint>();
}
