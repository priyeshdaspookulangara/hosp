// Timestamp used is just an example: 20250707000000
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCareERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key if it was based on old DoctorId (Guid) and linking to a different table
            // This depends on the very initial state of Appointment.DoctorId.
            // If it was a Guid not linking anywhere specific or to a non-existent Doctors table, this might not be needed.
            // For this exercise, I'll assume no FK needs dropping for DoctorId explicitly before altering.

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true, // Changed from not nullable to nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "Appointments",
                type: "nvarchar(450)", // To match ApplicationUser.Id type
                nullable: true,        // Changed to nullable string
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 30); // Default value as per entity constructor

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "Appointments",
                type: "nvarchar(100)", // Example length
                maxLength: 100,
                nullable: true);

            // Status column type change from string to int (for enum)
            // EF Core handles enum-to-int conversion by default. If it was string before, it needs to be altered.
            // If the old Status column was string:
            // migrationBuilder.DropColumn(name: "Status", table: "Appointments");
            // migrationBuilder.AddColumn<int>(name: "Status", table: "Appointments", type: "int", nullable: false, defaultValue: 1 /* Scheduled */);
            // If it was already int or a compatible type, just ensure it's `int`.
            // For safety, let's assume it was string and we are changing to int for the enum.
             migrationBuilder.AlterColumn<int>( // Or AddColumn if it was dropped
                name: "Status",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string), // Assuming previous type was string
                oldType: "nvarchar(max)",
                defaultValue: 1); // Default to Scheduled

            // Add new foreign key constraint for DoctorId to AspNetUsers table
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull); // Or Restrict, depending on desired behavior
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false, // Revert to not nullable
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,            // Revert to Guid, not nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // Revert Status column from int back to string
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1); // Remove default value if it was string before
        }
    }
}
