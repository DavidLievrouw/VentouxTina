using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;
using VentouxTina.Web.Domain.Services;

namespace VentouxTina.Web.Infrastructure.DataSources;

public class ProgressQueryService : IProgressDataSource
{
    private readonly VentouxTinaDbContext _db;

    public ProgressQueryService(VentouxTinaDbContext db)
    {
        _db = db;
    }

    public async Task<ProgressProjection?> ComputeProjectionAsync(CancellationToken ct = default)
    {
        var route = await _db.TripRoutes.AsNoTracking().FirstOrDefaultAsync(ct);

        if (route is null)
            return null;

        var entries = await _db
            .TripLogEntries.AsNoTracking()
            .OrderBy(e => e.Timestamp)
            .ToListAsync(ct);

        var polyline = RouteProjectionService.ParsePolyline(route.PolylineJson);
        return ProgressCalculator.Calculate(route, polyline, entries);
    }
}
