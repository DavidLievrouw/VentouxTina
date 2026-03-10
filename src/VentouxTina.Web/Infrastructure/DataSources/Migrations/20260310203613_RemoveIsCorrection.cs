using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentouxTina.Web.Infrastructure.DataSources.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsCorrection", table: "trip_log_entries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrection",
                table: "trip_log_entries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false
            );
        }
    }
}
