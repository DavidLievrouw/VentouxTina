using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Infrastructure.DataSources.Configurations;

public class ProjectContextConfiguration : IEntityTypeConfiguration<ProjectContext>
{
    public void Configure(EntityTypeBuilder<ProjectContext> builder)
    {
        builder.ToTable("project_contexts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Locale).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Headline).IsRequired().HasMaxLength(500);
        builder.Property(e => e.BodyText).IsRequired().HasColumnType("text");
        builder.Property(e => e.FundraisingGoalText).IsRequired().HasMaxLength(1000);
    }
}
