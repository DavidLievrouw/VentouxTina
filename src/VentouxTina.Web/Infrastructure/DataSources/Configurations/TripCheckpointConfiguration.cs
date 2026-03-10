using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources.Configurations;

public class TripCheckpointConfiguration : IEntityTypeConfiguration<TripCheckpoint>
{
    public void Configure(EntityTypeBuilder<TripCheckpoint> builder)
    {
        builder.ToTable("trip_checkpoints");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CumulativeDistanceKm).IsRequired().HasPrecision(10, 3);
        builder.Property(e => e.OrderIndex).IsRequired();
    }
}
