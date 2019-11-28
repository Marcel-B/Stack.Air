using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace com.b_velop.Stack.Air.Server.Migrations
{
    public partial class AddAirUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LinkValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Floor = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    Max = table.Column<double>(nullable: false),
                    Min = table.Column<double>(nullable: false),
                    ExternId = table.Column<string>(nullable: true),
                    Unit = table.Column<Guid>(nullable: false),
                    Location = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurePoints_Locations_Location",
                        column: x => x.Location,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasurePoints_Units_Unit",
                        column: x => x.Unit,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveMeasurePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastValue = table.Column<double>(nullable: false),
                    Point = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveMeasurePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveMeasurePoints_MeasurePoints_Point",
                        column: x => x.Point,
                        principalTable: "MeasurePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatteryStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    State = table.Column<bool>(nullable: false),
                    Point = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatteryStates_MeasurePoints_Point",
                        column: x => x.Point,
                        principalTable: "MeasurePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    Point = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasureValues_MeasurePoints_Point",
                        column: x => x.Point,
                        principalTable: "MeasurePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriorityStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Updated = table.Column<DateTimeOffset>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    State = table.Column<bool>(nullable: false),
                    Point = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriorityStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriorityStates_MeasurePoints_Point",
                        column: x => x.Point,
                        principalTable: "MeasurePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveMeasurePoints_Point",
                table: "ActiveMeasurePoints",
                column: "Point");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryStates_Point",
                table: "BatteryStates",
                column: "Point");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurePoints_Location",
                table: "MeasurePoints",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurePoints_Unit",
                table: "MeasurePoints",
                column: "Unit");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureValues_Point",
                table: "MeasureValues",
                column: "Point");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityStates_Point",
                table: "PriorityStates",
                column: "Point");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveMeasurePoints");

            migrationBuilder.DropTable(
                name: "AirUsers");

            migrationBuilder.DropTable(
                name: "BatteryStates");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "MeasureValues");

            migrationBuilder.DropTable(
                name: "PriorityStates");

            migrationBuilder.DropTable(
                name: "MeasurePoints");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
