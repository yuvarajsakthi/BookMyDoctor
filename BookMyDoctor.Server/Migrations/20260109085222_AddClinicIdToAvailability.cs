using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyDoctor.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicIdToAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Availabilities",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 10), new DateTime(2026, 1, 9, 8, 52, 16, 449, DateTimeKind.Utc).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 16), new DateTime(2026, 1, 9, 8, 52, 16, 449, DateTimeKind.Utc).AddTicks(4557) });

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_ClinicId",
                table: "Availabilities",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_Clinics_ClinicId",
                table: "Availabilities",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "ClinicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_Clinics_ClinicId",
                table: "Availabilities");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_ClinicId",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Availabilities");

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 7), new DateTime(2026, 1, 6, 5, 45, 21, 25, DateTimeKind.Utc).AddTicks(5900) });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 13), new DateTime(2026, 1, 6, 5, 45, 21, 25, DateTimeKind.Utc).AddTicks(5904) });
        }
    }
}
