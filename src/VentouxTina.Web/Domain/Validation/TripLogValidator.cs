using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Domain.Validation;

public static class TripLogValidator
{
    private static readonly string[] ValidActivities = ["running", "cycling", "walking"];

    public record ValidationResult(bool IsValid, string? Error);

    public static ValidationResult Validate(TripLogEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.EntryId))
            return new ValidationResult(false, "EntryId is vereist.");

        if (entry.Timestamp == default)
            return new ValidationResult(false, "Timestamp is ongeldig of ontbreekt.");

        if (entry.Timestamp > DateTime.UtcNow.AddDays(1))
            return new ValidationResult(false, "Timestamp ligt te ver in de toekomst.");

        if (entry.Kilometers < 0)
            return new ValidationResult(false, "Kilometers mag niet negatief zijn.");

        if (!ValidActivities.Contains(entry.Activity, StringComparer.OrdinalIgnoreCase))
            return new ValidationResult(
                false,
                $"Activiteit '{entry.Activity}' is ongeldig. Toegestaan: {string.Join(", ", ValidActivities)}."
            );

        return new ValidationResult(true, null);
    }
}
