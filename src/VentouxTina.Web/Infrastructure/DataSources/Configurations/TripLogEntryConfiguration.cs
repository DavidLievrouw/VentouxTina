using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources.Configurations;

public class TripLogEntryConfiguration : IEntityTypeConfiguration<TripLogEntry>
{
    public void Configure(EntityTypeBuilder<TripLogEntry> builder)
    {
        builder.ToTable("trip_log_entries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntryId).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.EntryId).IsUnique();
        builder.Property(e => e.Timestamp).IsRequired();
        builder.Property(e => e.Kilometers).IsRequired().HasPrecision(10, 3);
        builder.Property(e => e.Activity).IsRequired().HasMaxLength(50);
    }
}
