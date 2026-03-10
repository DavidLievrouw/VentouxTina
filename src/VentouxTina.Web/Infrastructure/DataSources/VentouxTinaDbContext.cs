using Microsoft.EntityFrameworkCore;
using VentouxTina.Web.Domain.Models;
using VentouxTina.Web.Infrastructure.DataSources.Configurations;

namespace VentouxTina.Web.Infrastructure.DataSources;

public class VentouxTinaDbContext : DbContext
{
    public VentouxTinaDbContext(DbContextOptions<VentouxTinaDbContext> options)
        : base(options) { }

    public DbSet<TripLogEntry> TripLogEntries => Set<TripLogEntry>();
    public DbSet<TripRoute> TripRoutes => Set<TripRoute>();
    public DbSet<TripCheckpoint> TripCheckpoints => Set<TripCheckpoint>();
    public DbSet<FundraisingGoal> FundraisingGoals => Set<FundraisingGoal>();
    public DbSet<ProjectContext> ProjectContexts => Set<ProjectContext>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TripLogEntryConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectContextConfiguration());
        modelBuilder.ApplyConfiguration(new TripRouteConfiguration());
        modelBuilder.ApplyConfiguration(new TripCheckpointConfiguration());
        modelBuilder.ApplyConfiguration(new FundraisingGoalConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
    }
}
