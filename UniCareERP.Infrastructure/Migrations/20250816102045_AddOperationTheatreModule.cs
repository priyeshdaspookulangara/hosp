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
            migrationBuilder.CreateTable(
                name: "OperationTheatres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTheatres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surgeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurgeryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationTheatreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrimarySurgeonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnesthetistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surgeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surgeries_Employees_AnesthetistId",
                        column: x => x.AnesthetistId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Surgeries_Employees_PrimarySurgeonId",
                        column: x => x.PrimarySurgeonId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Surgeries_OperationTheatres_OperationTheatreId",
                        column: x => x.OperationTheatreId,
                        principalTable: "OperationTheatres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Surgeries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurgeryChecklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurgeryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgeryChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurgeryChecklists_Surgeries_SurgeryId",
                        column: x => x.SurgeryId,
                        principalTable: "Surgeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Surgeries_AnesthetistId",
                table: "Surgeries",
                column: "AnesthetistId");

            migrationBuilder.CreateIndex(
                name: "IX_Surgeries_OperationTheatreId",
                table: "Surgeries",
                column: "OperationTheatreId");

            migrationBuilder.CreateIndex(
                name: "IX_Surgeries_PatientId",
                table: "Surgeries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Surgeries_PrimarySurgeonId",
                table: "Surgeries",
                column: "PrimarySurgeonId");

            migrationBuilder.CreateIndex(
                name: "IX_SurgeryChecklists_SurgeryId",
                table: "SurgeryChecklists",
                column: "SurgeryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurgeryChecklists");

            migrationBuilder.DropTable(
                name: "Surgeries");

            migrationBuilder.DropTable(
                name: "OperationTheatres");
        }
    }
}
