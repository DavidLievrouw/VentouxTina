using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources;

public interface ITripLogWriteService
{
    /// <summary>Returns the timestamp of the most recent trip log entry, or null if none exist.</summary>
    Task<DateTime?> GetLatestTimestampAsync(CancellationToken ct = default);

    Task AddEntryAsync(TripLogEntry entry, CancellationToken ct = default);
}

public class TripLogWriteService : ITripLogWriteService
{
    private readonly Func<VentouxTinaDbContext> _dbContextFactory;

    public TripLogWriteService(Func<VentouxTinaDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<DateTime?> GetLatestTimestampAsync(CancellationToken ct = default)
    {
        await using var db = _dbContextFactory();
        return await db
            .TripLogEntries.AsNoTracking()
            .OrderByDescending(e => e.Timestamp)
            .Select(e => (DateTime?)e.Timestamp)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task AddEntryAsync(TripLogEntry entry, CancellationToken ct = default)
    {
        await using var db = _dbContextFactory();
        db.TripLogEntries.Add(entry);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
