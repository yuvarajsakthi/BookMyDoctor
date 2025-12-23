using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyDoctor.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorIsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Doctors",
                newName: "IsApproved");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CloseTime",
                table: "Clinics",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HolidayDates",
                table: "Clinics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpenTime",
                table: "Clinics",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "Clinics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 1,
                columns: new[] { "CloseTime", "HolidayDates", "OpenTime", "WorkingDays" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 2,
                columns: new[] { "CloseTime", "HolidayDates", "OpenTime", "WorkingDays" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "HolidayDates",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Clinics");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "Doctors",
                newName: "IsVerified");

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "hashedpassword123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "hashedpassword456");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "hashedpassword789");
        }
    }
}
