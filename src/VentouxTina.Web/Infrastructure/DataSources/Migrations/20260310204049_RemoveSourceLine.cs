using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentouxTina.Web.Infrastructure.DataSources.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSourceLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "SourceLine", table: "trip_log_entries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceLine",
                table: "trip_log_entries",
                type: "int",
                nullable: true
            );
        }
    }
}
