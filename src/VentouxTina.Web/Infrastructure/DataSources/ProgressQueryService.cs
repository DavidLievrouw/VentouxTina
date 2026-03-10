using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;
using VentouxTina.Web.Domain.Services;

namespace VentouxTina.Web.Infrastructure.DataSources;

public class ProgressQueryService : IProgressDataSource
{
    private readonly Func<VentouxTinaDbContext> _dbContextFactory;

    public ProgressQueryService(Func<VentouxTinaDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<ProgressProjection?> ComputeProjectionAsync(CancellationToken ct = default)
    {
        await using var db = _dbContextFactory();
        var route = await db
            .TripRoutes.AsNoTracking()
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (route is null)
            return null;

        var entries = await db
            .TripLogEntries.AsNoTracking()
            .OrderBy(e => e.Timestamp)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var polyline = RouteProjectionService.ParsePolyline(route.PolylineJson);
        return ProgressCalculator.Calculate(route, polyline, entries);
    }
}
