using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources.Configurations;

public class TripRouteConfiguration : IEntityTypeConfiguration<TripRoute>
{
    public void Configure(EntityTypeBuilder<TripRoute> builder)
    {
        builder.ToTable("trip_routes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RouteId).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.RouteId).IsUnique();
        builder.Property(e => e.StartName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.EndName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.TotalDistanceKm).IsRequired().HasPrecision(10, 3);
        builder.Property(e => e.PolylineJson).IsRequired().HasColumnType("longtext");
        builder
            .HasMany(e => e.Checkpoints)
            .WithOne(c => c.TripRoute)
            .HasForeignKey(c => c.TripRouteId);
    }
}
