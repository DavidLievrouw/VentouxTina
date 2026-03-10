namespace VentouxTina.Web.Domain.Models;

public class TripLogEntry
{
    public int Id { get; set; }
    public string EntryId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Kilometers { get; set; }
    public string Activity { get; set; } = string.Empty;
    public int? SourceLine { get; set; }
    public bool IsCorrection { get; set; }
}
