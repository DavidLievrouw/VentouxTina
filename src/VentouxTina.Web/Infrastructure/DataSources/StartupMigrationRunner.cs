using Microsoft.EntityFrameworkCore;

namespace VentouxTina.Web.Infrastructure.DataSources;

public static class StartupMigrationRunner
{
    public static async Task RunAsync(IServiceProvider services, ILogger logger)
    {
        var dbContextFactory = services.GetRequiredService<Func<VentouxTinaDbContext>>();
        await using var db = dbContextFactory();

        try
        {
            var pending = await db.Database.GetPendingMigrationsAsync();
            var pendingList = pending.ToList();

            if (pendingList.Count == 0)
            {
                logger.LogInformation("Database is up-to-date. No pending migrations.");
                return;
            }

            logger.LogInformation(
                "Applying {Count} pending migration(s): {Migrations}",
                pendingList.Count,
                string.Join(", ", pendingList)
            );

            await db.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}
