using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources;

public interface ITripLogDataSource
{
    Task<IReadOnlyList<TripLogEntry>> GetEntriesAsync(
        int? limit = null,
        CancellationToken ct = default
    );
}

public class TripLogQueryService : ITripLogDataSource
{
    private readonly Func<VentouxTinaDbContext> _dbContextFactory;

    public TripLogQueryService(Func<VentouxTinaDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<TripLogEntry>> GetEntriesAsync(
        int? limit = null,
        CancellationToken ct = default
    )
    {
        await using var db = _dbContextFactory();
        var query = db.TripLogEntries.AsNoTracking().OrderBy(e => e.Timestamp);

        if (limit > 0)
        {
            return await query.Take(limit.Value).ToListAsync(ct).ConfigureAwait(false);
        }

        return await query.ToListAsync(ct).ConfigureAwait(false);
    }
}
