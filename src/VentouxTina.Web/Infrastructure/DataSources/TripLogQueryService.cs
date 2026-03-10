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
    private readonly VentouxTinaDbContext _db;

    public TripLogQueryService(VentouxTinaDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TripLogEntry>> GetEntriesAsync(
        int? limit = null,
        CancellationToken ct = default
    )
    {
        var query = _db.TripLogEntries.AsNoTracking().OrderBy(e => e.Timestamp);

        if (limit > 0)
            return await query.Take(limit.Value).ToListAsync(ct);

        return await query.ToListAsync(ct);
    }
}
