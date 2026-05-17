using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkDayDateToStaffResidentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDayDate",
                table: "StaffResidentStatuses",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                columns: new[] { "StaffID", "ResidentStatusID", "WorkDayDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.DropColumn(
                name: "WorkDayDate",
                table: "StaffResidentStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                columns: new[] { "StaffID", "ResidentStatusID" });
        }
    }
}
