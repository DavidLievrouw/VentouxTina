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
    private readonly Func<VentouxTinaDbContext> _dbContextFactory;

    public ProjectContextQueryService(Func<VentouxTinaDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<(ProjectContext? Context, FundraisingGoal? Goal)> GetContextAsync(
        CancellationToken ct = default
    )
    {
        using (var db = _dbContextFactory())
        {
            var context = await db
                .ProjectContexts.AsNoTracking()
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            var goal = await db
                .FundraisingGoals.AsNoTracking()
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            return (context, goal);
        }
    }
}
