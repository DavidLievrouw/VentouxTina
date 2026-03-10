using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources;

public interface ITripLogWriteService
{
    Task<DateTime?> GetLatestTimestampAsync(CancellationToken ct = default);

    Task AddEntryAsync(TripLogEntry entry, CancellationToken ct = default);

    Task<bool> DeleteEntryAsync(string entryId, CancellationToken ct = default);
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

    public async Task<bool> DeleteEntryAsync(string entryId, CancellationToken ct = default)
    {
        await using var db = _dbContextFactory();
        var entry = await db
            .TripLogEntries.FirstOrDefaultAsync(e => e.EntryId == entryId, ct)
            .ConfigureAwait(false);
        if (entry is null)
        {
            return false;
        }

        db.TripLogEntries.Remove(entry);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        return true;
    }
}
