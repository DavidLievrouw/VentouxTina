using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources.Configurations;

public class FundraisingGoalConfiguration : IEntityTypeConfiguration<FundraisingGoal>
{
    public void Configure(EntityTypeBuilder<FundraisingGoal> builder)
    {
        builder.ToTable("fundraising_goals");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.OrganizationName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.GoalAmountEur).IsRequired().HasPrecision(10, 2);
        builder.Property(e => e.Audience).HasMaxLength(200);
    }
}
