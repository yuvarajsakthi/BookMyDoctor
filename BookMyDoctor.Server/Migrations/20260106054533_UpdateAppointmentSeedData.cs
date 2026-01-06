using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookMyDoctor.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AppointmentType",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 2);

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentDate", "AppointmentType", "ClinicId", "CreatedAt", "DoctorId", "EndTime", "PatientId", "Reason", "StartTime", "Status" },
                values: new object[,]
                {
                    { 2, new DateOnly(2026, 1, 7), 0, 1, new DateTime(2026, 1, 6, 5, 45, 21, 25, DateTimeKind.Utc).AddTicks(5900), 2, new TimeOnly(14, 30, 0), 3, "Follow-up consultation", new TimeOnly(14, 0, 0), 1 },
                    { 3, new DateOnly(2026, 1, 13), 0, 1, new DateTime(2026, 1, 6, 5, 45, 21, 25, DateTimeKind.Utc).AddTicks(5904), 2, new TimeOnly(11, 30, 0), 3, "Routine examination", new TimeOnly(11, 0, 0), 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentType",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 1);
        }
    }
}
