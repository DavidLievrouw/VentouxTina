using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentouxTina.Web.Infrastructure.DataSources.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "fundraising_goals",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        OrganizationName = table
                            .Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        GoalAmountEur = table.Column<decimal>(
                            type: "decimal(10,2)",
                            nullable: false,
                            precision: 10,
                            scale: 2
                        ),
                        IsFundraiser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                        Audience = table
                            .Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table => table.PrimaryKey("PK_fundraising_goals", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "project_contexts",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Locale = table
                            .Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Headline = table
                            .Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        BodyText = table
                            .Column<string>(type: "text", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        FundraisingGoalText = table
                            .Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table => table.PrimaryKey("PK_project_contexts", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "trip_log_entries",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        EntryId = table
                            .Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        Kilometers = table.Column<decimal>(
                            type: "decimal(10,3)",
                            nullable: false,
                            precision: 10,
                            scale: 3
                        ),
                        Activity = table
                            .Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        SourceLine = table.Column<int>(type: "int", nullable: true),
                        IsCorrection = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                    },
                    constraints: table => table.PrimaryKey("PK_trip_log_entries", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "trip_routes",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        RouteId = table
                            .Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        StartName = table
                            .Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        EndName = table
                            .Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        TotalDistanceKm = table.Column<decimal>(
                            type: "decimal(10,3)",
                            nullable: false,
                            precision: 10,
                            scale: 3
                        ),
                        PolylineJson = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table => table.PrimaryKey("PK_trip_routes", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "trip_checkpoints",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        TripRouteId = table.Column<int>(type: "int", nullable: false),
                        Name = table
                            .Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        CumulativeDistanceKm = table.Column<decimal>(
                            type: "decimal(10,3)",
                            nullable: false,
                            precision: 10,
                            scale: 3
                        ),
                        Latitude = table.Column<double>(type: "double", nullable: false),
                        Longitude = table.Column<double>(type: "double", nullable: false),
                        OrderIndex = table.Column<int>(type: "int", nullable: false),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_trip_checkpoints", x => x.Id);
                        table.ForeignKey(
                            name: "FK_trip_checkpoints_trip_routes_TripRouteId",
                            column: x => x.TripRouteId,
                            principalTable: "trip_routes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_trip_checkpoints_TripRouteId",
                table: "trip_checkpoints",
                column: "TripRouteId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_trip_log_entries_EntryId",
                table: "trip_log_entries",
                column: "EntryId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_trip_routes_RouteId",
                table: "trip_routes",
                column: "RouteId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "fundraising_goals");

            migrationBuilder.DropTable(name: "project_contexts");

            migrationBuilder.DropTable(name: "trip_checkpoints");

            migrationBuilder.DropTable(name: "trip_log_entries");

            migrationBuilder.DropTable(name: "trip_routes");
        }
    }
}
