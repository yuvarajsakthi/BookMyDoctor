using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyDoctor.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt", "Status" },
                values: new object[] { new DateOnly(2026, 1, 12), new DateTime(2026, 1, 11, 11, 2, 6, 567, DateTimeKind.Utc).AddTicks(8664), 3 });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 18), new DateTime(2026, 1, 11, 11, 2, 6, 567, DateTimeKind.Utc).AddTicks(8667) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt", "Status" },
                values: new object[] { new DateOnly(2026, 1, 10), new DateTime(2026, 1, 9, 8, 52, 16, 449, DateTimeKind.Utc).AddTicks(4550), 1 });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateOnly(2026, 1, 16), new DateTime(2026, 1, 9, 8, 52, 16, 449, DateTimeKind.Utc).AddTicks(4557) });
        }
    }
}
