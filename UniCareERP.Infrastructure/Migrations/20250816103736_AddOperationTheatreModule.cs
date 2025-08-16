using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCareERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationTheatreModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SurgicalTeamId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OperationTheatres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Equipment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTheatres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurgicalProcedures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredEquipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgicalProcedures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurgicalTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgicalTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationTheatreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurgicalProcedureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurgicalTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTSchedules_OperationTheatres_OperationTheatreId",
                        column: x => x.OperationTheatreId,
                        principalTable: "OperationTheatres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OTSchedules_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OTSchedules_SurgicalProcedures_SurgicalProcedureId",
                        column: x => x.SurgicalProcedureId,
                        principalTable: "SurgicalProcedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OTSchedules_SurgicalTeams_SurgicalTeamId",
                        column: x => x.SurgicalTeamId,
                        principalTable: "SurgicalTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SurgicalTeamId",
                table: "AspNetUsers",
                column: "SurgicalTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OTSchedules_OperationTheatreId",
                table: "OTSchedules",
                column: "OperationTheatreId");

            migrationBuilder.CreateIndex(
                name: "IX_OTSchedules_PatientId",
                table: "OTSchedules",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_OTSchedules_SurgicalProcedureId",
                table: "OTSchedules",
                column: "SurgicalProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_OTSchedules_SurgicalTeamId",
                table: "OTSchedules",
                column: "SurgicalTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SurgicalTeams_SurgicalTeamId",
                table: "AspNetUsers",
                column: "SurgicalTeamId",
                principalTable: "SurgicalTeams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SurgicalTeams_SurgicalTeamId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "OTSchedules");

            migrationBuilder.DropTable(
                name: "OperationTheatres");

            migrationBuilder.DropTable(
                name: "SurgicalProcedures");

            migrationBuilder.DropTable(
                name: "SurgicalTeams");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SurgicalTeamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SurgicalTeamId",
                table: "AspNetUsers");
        }
    }
}
