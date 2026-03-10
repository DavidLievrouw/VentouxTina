using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources;

public interface IUserRepository
{
    Task<AppUser?> FindByUsernameAsync(string username, CancellationToken ct = default);
}

public class UserRepository : IUserRepository
{
    private readonly Func<VentouxTinaDbContext> _dbContextFactory;

    public UserRepository(Func<VentouxTinaDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<AppUser?> FindByUsernameAsync(string username, CancellationToken ct = default)
    {
        await using var db = _dbContextFactory();
        return await db
            .AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, ct)
            .ConfigureAwait(false);
    }
}
