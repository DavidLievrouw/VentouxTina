using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources;

public interface IProjectContextDataSource
{
    Task<(ProjectContext? Context, FundraisingGoal? Goal)> GetContextAsync(
        CancellationToken ct = default
    );
}

public class ProjectContextQueryService : IProjectContextDataSource
{
    private readonly VentouxTinaDbContext _db;

    public ProjectContextQueryService(VentouxTinaDbContext db)
    {
        _db = db;
    }

    public async Task<(ProjectContext? Context, FundraisingGoal? Goal)> GetContextAsync(
        CancellationToken ct = default
    )
    {
        var context = await _db
            .ProjectContexts.AsNoTracking()
            .FirstOrDefaultAsync(ct);

        var goal = await _db
            .FundraisingGoals.AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return (context, goal);
    }
}
